# KOL/KOC Hiring Marketplace – Cây sơ đồ hình thành web (Site Map + Modules)

## 1) Site Map (cây trang web)

```
Public
├─ Trang chủ / Landing
├─ Đăng ký / Đăng nhập
├─ Tìm kiếm KOL (Search)
│  ├─ Bộ lọc: category, tag, city, min/max budget, rating, platform
│  └─ Trang chi tiết KOL
│     ├─ Portfolio
│     ├─ Social accounts
│     └─ Rate card (bảng giá)
└─ FAQ / Điều khoản

Customer (Brand) Dashboard
├─ Hồ sơ cá nhân
├─ Yêu thích (Favorite KOLs)
├─ Booking Requests (Yêu cầu booking)
│  ├─ Tạo yêu cầu (booking_requests + booking_request_items)
│  ├─ Đàm phán / chat
│  └─ Chấp nhận -> Booking
├─ Bookings (Đơn booking)
│  ├─ Timeline trạng thái: pending_payment → paid → in_progress → completed/refunded
│  ├─ Meetings (lịch trao đổi)
│  ├─ Deliverables (bài/clip phải bàn giao)
│  ├─ Contracts (hợp đồng) + chữ ký
│  ├─ Disputes (khiếu nại)
│  └─ Reviews (đánh giá KOL)
├─ Payments
│  ├─ Invoice
│  ├─ Payment Intent
│  ├─ Payments / Refunds
│  └─ Coupons / Subscription
└─ Notifications

KOL/KOC Dashboard
├─ Hồ sơ KOL (kol_profiles)
│  ├─ Social accounts (kol_social_accounts)
│  ├─ Portfolio (kol_portfolios)
│  ├─ Categories/Tags (kol_category_map, kol_tag_map)
│  └─ Rate cards (rate_cards + rate_card_items)
├─ Availability (kol_availability_slots)
├─ Booking Requests nhận được
├─ Bookings đang chạy
│  ├─ Deliverables: submit / revise / approve
│  ├─ Meetings
│  ├─ Chat
│  └─ Contracts + ký
├─ Wallet
│  ├─ Balance / Locked balance
│  ├─ Ledger entries
│  └─ Payout requests
└─ Notifications

Admin Portal
├─ Users / Roles
├─ KOL Verification
├─ Booking/Payments monitoring
├─ Disputes center
├─ Reports moderation (chat/content)
├─ Payout processing
├─ System settings
├─ Audit logs
└─ Reporting dashboard
```

## 2) Backend Modules (bounded contexts) → DB tables

```
Identity
├─ users, roles, user_roles, sessions

KOL Profile
├─ kol_profiles, kol_social_accounts, kol_portfolios
├─ kol_categories, kol_category_map
├─ tags, kol_tag_map
└─ rate_cards, rate_card_items, rate_card_history

Booking
├─ booking_requests, booking_request_items
└─ bookings, booking_items

Chat + Files
├─ files
├─ chat_conversations, chat_members, chat_messages

Meetings
├─ kol_availability_slots
├─ meetings, meeting_participants

Payments
├─ invoices
├─ payment_methods
├─ payment_intents, payments, refunds
├─ coupons, coupon_usages
└─ subscriptions: subscription_plans, user_subscriptions

Contracts + Deliverables
├─ contracts, contract_signatures
└─ deliverables, deliverable_attachments

Wallet + Payout
├─ user_wallets
├─ wallet_ledger
└─ payout_requests

Disputes + Moderation
├─ disputes
└─ reports

System
├─ system_settings
├─ notifications
└─ admin_audit_logs

Reporting Views
├─ view_kol_search
└─ view_platform_revenue
```

## 3) “Luồng chính” (happy path) để triển khai web

1) Customer tìm KOL → xem profile (view_kol_search + detail joins)
2) Customer tạo booking request (booking_requests + booking_request_items)
3) Chat & đàm phán → accepted
4) Tạo booking + booking_items + contract draft
5) Customer thanh toán: invoice → payment_intent → payment → booking paid
6) Meetings + Deliverables chạy trong booking
7) Hoàn thành: review + cập nhật rating
8) KOL nhận tiền vào wallet_ledger + payout_requests (nếu rút)
9) Nếu có vấn đề: disputes + refunds (nếu cần)
