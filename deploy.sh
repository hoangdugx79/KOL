#!/bin/bash
export DEBIAN_FRONTEND=noninteractive

# ============================================================
# SCRIPT DEPLOY .NET CORE LÊN VPS (Ubuntu/Debian)
# App: KOL/KOC Marketplace
# DB:  Azure SQL Database
# ============================================================

# --- CẤU HÌNH (BẮT BUỘC THAY ĐỔI) ---
REPO="hoangdugx79/KOL"   # Ví dụ: dungadmin/KOL_KOC_WEB
DOMAIN="kol.anclick.id.vn"
PROJECT_NAME="KOL_KOC_TAAA"
DOTNET_VERSION="10.0"                  # Phiên bản .NET SDK

# Các thư mục quan trọng
APP_DIR="$HOME/kol_koc_web"
SOURCE_DIR="$HOME/kol_koc_source_temp"
BUILD_DIR="$HOME/kol_koc_build_temp"
DATA_DIR="$HOME/kol_koc_data"

log() { echo "👉 [$(date '+%H:%M:%S')] $1"; }

log "🚀 BẮT ĐẦU QUÁ TRÌNH DEPLOY"

# ============================================================
# BƯỚC 0: KIỂM TRA & CÀI ĐẶT CÔNG CỤ CẦN THIẾT
# ============================================================
log "--- Bước 0: Kiểm tra môi trường VPS ---"

# 0.1 Cập nhật package list
sudo apt-get update -y

# 0.2 Git
if ! command -v git &>/dev/null; then
    log "Cài đặt Git..."
    sudo apt-get install -y git
fi
log "✅ Git: $(git --version)"

# 0.3 .NET SDK
if ! command -v dotnet &>/dev/null || ! dotnet --list-sdks | grep -q "^${DOTNET_VERSION}"; then
    log "Cài đặt .NET SDK ${DOTNET_VERSION}..."
    # Thêm Microsoft package repository
    wget -q https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
    sudo dpkg -i /tmp/packages-microsoft-prod.deb
    rm /tmp/packages-microsoft-prod.deb
    sudo apt-get update -y
    sudo apt-get install -y dotnet-sdk-${DOTNET_VERSION}
fi
log "✅ .NET SDK: $(dotnet --version)"

# 0.4 Node.js + PM2 (dùng PM2 để quản lý process .NET)
if ! command -v node &>/dev/null; then
    log "Cài đặt Node.js..."
    curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
    sudo apt-get install -y nodejs
fi
log "✅ Node.js: $(node --version)"

if ! command -v pm2 &>/dev/null; then
    log "Cài đặt PM2..."
    sudo npm install -g pm2
    pm2 startup systemd -u $USER --hp $HOME 2>/dev/null || true
fi
log "✅ PM2: $(pm2 --version)"

# 0.5 Nginx
if ! command -v nginx &>/dev/null; then
    log "Cài đặt Nginx..."
    sudo apt-get install -y nginx
    sudo systemctl enable nginx
    sudo systemctl start nginx
fi
log "✅ Nginx: $(nginx -v 2>&1)"

# 0.6 SSL: Dùng Cloudflare proxy → không cần Certbot
# Cloudflare tự cấp SSL giữa client ↔ Cloudflare
# Cloudflare → VPS dùng HTTP (Flexible) hoặc Full mode
log "✅ SSL: Sử dụng Cloudflare (không cần Certbot)"

# 0.7 EF Core tools
dotnet tool install --global dotnet-ef >/dev/null 2>&1 || true
export PATH="$PATH:$HOME/.dotnet/tools"

log "✅ Tất cả công cụ đã sẵn sàng!"

# ============================================================
# BƯỚC 1: CẤU HÌNH NGINX (chỉ tạo 1 lần, lần sau bỏ qua)
# ============================================================
NGINX_CONF="/etc/nginx/sites-available/$DOMAIN"
if [ ! -f "$NGINX_CONF" ]; then
    log "--- Bước 1: Cấu hình Nginx cho $DOMAIN ---"

    sudo tee "$NGINX_CONF" > /dev/null <<NGINX
server {
    listen 80;
    server_name $DOMAIN;

    # Giới hạn upload file 50MB
    client_max_body_size 50M;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;

        # Timeout cho các request dài (SignalR, upload)
        proxy_read_timeout 300s;
        proxy_connect_timeout 60s;
        proxy_send_timeout 300s;
    }

    # Cache static files
    location ~* \.(css|js|jpg|jpeg|png|gif|ico|svg|woff|woff2|ttf|eot)$ {
        proxy_pass http://localhost:5000;
        proxy_set_header Host \$host;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
NGINX

    # Kích hoạt site
    sudo ln -sf "$NGINX_CONF" /etc/nginx/sites-enabled/
    sudo rm -f /etc/nginx/sites-enabled/default

    # Test cấu hình nginx
    if sudo nginx -t; then
        sudo systemctl reload nginx
        log "✅ Nginx đã cấu hình xong cho $DOMAIN"
    else
        log "❌ Lỗi cấu hình Nginx! Kiểm tra lại."
        exit 1
    fi

    # SSL do Cloudflare xử lý, không cần Certbot
    log "✅ SSL sẽ do Cloudflare xử lý (đặt SSL mode = Flexible hoặc Full trên Cloudflare Dashboard)"
else
    log "✅ Nginx đã được cấu hình trước đó, bỏ qua."
fi

# ============================================================
# BƯỚC 2: TẢI CODE TỪ GITHUB
# ============================================================
log "--- Bước 2: Tải code mới từ Git ---"
mkdir -p "$DATA_DIR/uploads"
rm -rf "$SOURCE_DIR"
mkdir -p "$SOURCE_DIR"

if git clone git@github.com:$REPO.git "$SOURCE_DIR"; then
    cd "$SOURCE_DIR"
    log "✅ Clone code thành công!"
else
    log "❌ Lỗi Git Clone. Dừng deploy."
    exit 1
fi

# ============================================================
# BƯỚC 3: BUILD & PUBLISH
# ============================================================
log "--- Bước 3: Build & Publish (.NET) ---"
rm -rf "$BUILD_DIR"

if dotnet publish $PROJECT_NAME/$PROJECT_NAME.csproj -c Release -o "$BUILD_DIR"; then
    log "✅ Build & Publish thành công!"
else
    log "❌ Build thất bại! Web cũ vẫn an toàn. Hủy deploy."
    exit 1
fi

# ============================================================
# BƯỚC 4: TẠO FILE CẤU HÌNH PRODUCTION
# ============================================================
log "--- Bước 4: Tạo cấu hình Production ---"

cat > "$BUILD_DIR/appsettings.Production.json" <<EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:dung-clinic-server.database.windows.net,1433;Initial Catalog=ClinicDB;Persist Security Info=False;User ID=dungadmin;Password=Anhuang279?;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Gemini": {
    "ApiKey": "AIzaSyDaa-YoDbAIEYnO3a6EvJaGTrrpbFn9Dh0"
  },
  "Momo": {
    "PartnerCode": "YOUR_PARTNER_CODE",
    "AccessKey": "YOUR_ACCESS_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "BaseUrl": "https://test-payment.momo.vn",
    "ReturnUrl": "https://$DOMAIN/Payment/MomoReturn",
    "NotifyUrl": "https://$DOMAIN/Payment/MomoNotify"
  }
}
EOF

# Cấu hình PM2
cat > "$BUILD_DIR/ecosystem.config.js" <<EOF
module.exports = {
  apps: [{
    name: '$PROJECT_NAME',
    script: 'dotnet',
    args: '$PROJECT_NAME.dll --urls "http://localhost:5000"',
    cwd: '$APP_DIR',
    exec_mode: 'fork',
    instances: 1,
    wait_ready: false,
    autorestart: true,
    max_restarts: 10,
    restart_delay: 5000,
    env: {
      ASPNETCORE_ENVIRONMENT: 'Production',
      DOTNET_PRINT_TELEMETRY_MESSAGE: 'false'
    }
  }]
};
EOF

# ============================================================
# BƯỚC 5: TRÁO ĐỔI CODE (SWAP) & KHỞI ĐỘNG
# ============================================================
log "--- Bước 5: Tráo đổi code cũ → mới ---"
rm -rf "$APP_DIR"
mv "$BUILD_DIR" "$APP_DIR"

# Symlink thư mục uploads (giữ file upload qua các lần deploy)
mkdir -p "$APP_DIR/wwwroot"
ln -sf "$DATA_DIR/uploads" "$APP_DIR/wwwroot/uploads"

# ============================================================
# BƯỚC 6: KHỞI ĐỘNG APP
# ============================================================
log "--- Bước 6: Khởi động App với PM2 ---"
cd "$APP_DIR"
pm2 startOrReload ecosystem.config.js --update-env
pm2 save

# Dọn dẹp thư mục tạm
rm -rf "$SOURCE_DIR"

log "🎉 DEPLOY HOÀN TẤT!"
log "🌐 Website: https://$DOMAIN"
log "📋 Kiểm tra app: pm2 status"
log "📋 Xem log app:  pm2 logs $PROJECT_NAME"
log "📋 Xem log nginx: sudo tail -f /var/log/nginx/error.log"