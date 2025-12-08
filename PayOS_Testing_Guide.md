# PayOS Payment Testing Guide

## ✅ Fixes Applied

1. ✅ Updated `appsettings.json` - PayOS ReturnUrl now points to API backend
2. ✅ Created `appsettings.Development.json` with Frontend URLs
3. ✅ Created `appsettings.Production.json` template
4. ✅ Removed duplicate [Authorize] attribute

## 🧪 How to Test Locally

### Prerequisites
- Ensure your API is running on `https://localhost:7050`
- Ensure your frontend is running on `http://localhost:5173`
- Have a valid Bearer token for authentication

### Step 1: Create a Booking
First, you need a valid booking to create a payment for.

```bash
POST https://localhost:7050/api/v1/booking/create
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "freelancerId": "VALID_FREELANCER_GUID",
  "petId": "VALID_PET_GUID",
  "serviceId": "VALID_SERVICE_GUID",
  "startTime": "2025-12-10T10:00:00Z",
  "endTime": "2025-12-10T12:00:00Z",
  "location": "123 Test Street",
  "note": "Test booking for payment"
}
```

Save the returned `bookingId`.

### Step 2: Create Payment

```bash
POST https://localhost:7050/api/v1/payment/create
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "bookingId": "YOUR_BOOKING_ID_FROM_STEP_1",
  "method": "PayOS",
  "description": "Test payment for booking"
}
```

**Expected Response:**
```json
{
  "success": true,
  "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?...",
  "message": "Payment URL created successfully"
}
```

### Step 3: Complete Payment

1. Copy the `paymentUrl` from the response
2. Open it in your browser (or click from your frontend)
3. You'll be redirected to PayOS sandbox payment page
4. Use PayOS test card:
   - Card Number: `9704 0000 0000 0018`
   - Cardholder: `NGUYEN VAN A`
   - Expiry: Any future date (e.g., `12/25`)
   - OTP: `123456`

### Step 4: Verify Callback Flow

After completing payment, observe:

1. **Browser redirects to:** `https://localhost:7050/api/v1/payment/payos-callback?orderCode=XXXXXXXXX&...`
2. **Check API logs** - you should see:
   ```
   PayOS callback received with X parameters
   Payment updated successfully. BookingId: XXX, TransactionId: XXX
   ```
3. **Browser final redirect to:** `http://localhost:5173/payment-success?bookingId=XXX&transactionId=XXX`

### Step 5: Verify Database

Check your Payment table:
```sql
SELECT * FROM "Payment" WHERE "BookingId" = 'YOUR_BOOKING_ID';
```

Expected:
- `Status` = 1 (Success)
- `TransactionId` = orderCode from PayOS
- `Amount` = booking total price

Check your Booking table:
```sql
SELECT * FROM "Booking" WHERE "BookingId" = 'YOUR_BOOKING_ID';
```

Expected:
- `IsPaid` = true
- `Status` = 2 (Confirmed)

### Step 6: Check Payment Status via API

```bash
GET https://localhost:7050/api/v1/payment/booking/YOUR_BOOKING_ID/status
Authorization: Bearer YOUR_TOKEN
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": "...",
    "bookingId": "...",
    "status": "Success",
    "amount": 200000,
    "method": "PayOS",
    "createdAt": "2025-12-08T..."
  }
}
```

## 🚨 Common Issues

### Issue 1: SSL Certificate Error
**Problem:** PayOS requires HTTPS but localhost cert is not trusted

**Solution:**
```bash
dotnet dev-certs https --trust
```

### Issue 2: Callback Not Received
**Problem:** PayOS redirects but callback endpoint not hit

**Check:**
1. Verify `PayOS:ReturnUrl` in appsettings.json is correct
2. Check API is running on the specified port
3. Look at browser network tab - does the redirect happen?
4. Check API logs for any errors

### Issue 3: CORS Error
**Problem:** Frontend can't call payment API

**Solution:** Ensure CORS is configured in `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...

app.UseCors("AllowFrontend");
```

### Issue 4: Payment Not Found
**Problem:** `GetPaymentByTransactionIdAsync` returns null

**Check:**
1. Verify TransactionId was saved during CreatePayment
2. Check database: `SELECT * FROM "Payment" WHERE "TransactionId" = 'orderCode'`
3. Ensure orderCode from callback matches TransactionId in database

## 📊 Success Indicators

✅ Payment record created with Status = Pending
✅ PayOS payment URL generated
✅ User redirected to PayOS sandbox
✅ Payment completed on PayOS
✅ Callback endpoint called at `/api/v1/payment/payos-callback`
✅ Payment status updated to Success
✅ Booking status updated to Confirmed, IsPaid = true
✅ User redirected to frontend success page
✅ Frontend receives bookingId and transactionId

## 🔄 Full Flow Diagram

```
Customer                 Frontend              API               PayOS
   |                        |                   |                  |
   |---Click Pay----------->|                   |                  |
   |                        |---POST /create--->|                  |
   |                        |                   |--Create Link---->|
   |                        |<--paymentUrl------|                  |
   |<--Redirect-------------|                   |                  |
   |                                            |                  |
   |---Complete Payment------------------------------------------->|
   |                                            |                  |
   |<--Redirect (with orderCode)----------------|<--Callback-------|
   |                                            |                  |
   |                                            |--Verify Status-->|
   |                                            |<--PAID-----------|
   |                                            |                  |
   |                                     [Update DB]              |
   |                                            |                  |
   |<--Redirect to success-------------------- |                  |
   |  (bookingId + transactionId)              |                  |
```

## 🚀 Ready for Production?

Before deploying:

- [ ] Update `appsettings.Production.json` with production PayOS credentials
- [ ] Replace all localhost URLs with production domains
- [ ] Enable HTTPS on your production server
- [ ] Test with PayOS sandbox first
- [ ] Set up proper logging/monitoring
- [ ] Configure environment variables for secrets
- [ ] Test error scenarios (payment failed, cancelled, timeout)
- [ ] Verify webhook handling (if using PayOS webhooks)

## 📞 Support

If payment fails:
1. Check API logs (`_logger.LogError`)
2. Check PayOS dashboard for transaction status
3. Verify all credentials are correct
4. Test orderCode manually: `GET /api/v1/payment/{bookingId}/status`

