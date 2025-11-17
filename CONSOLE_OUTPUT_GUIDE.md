# Console Output Guide - AIS Stream Debug Logs

## 🎯 Apa yang Akan Anda Lihat di Console

Sekarang aplikasi akan menampilkan **detailed console logs** untuk setiap tahap koneksi dan data.

---

## 📋 Expected Console Output

### 1. **Saat Aplikasi Start**

```
=== AIS Service Initialization Started ===
[14:23:45.000] Creating AIS Service instance...
[14:23:45.010] Subscribing to events...
[14:23:45.015] Starting AIS Stream connection...
[14:23:45.020] 🔌 Attempting to start AIS stream...
```

---

### 2. **WebSocket Connection Phase**

```
[14:23:45.100] [AIS] 🔌 Connecting to WebSocket...
[14:23:45.101] [AIS] URL: wss://stream.aisstream.io/v0/stream
[14:23:46.234] [AIS] ✅ WebSocket connected successfully!
[14:23:46.235] [AIS] WebSocket State: Open
[14:23:46.236] 🔌 CONNECTION STATUS CHANGED: CONNECTED ✅
```

---

### 3. **Subscription Phase**

```
[14:23:46.240] [AIS] 📤 Sending subscription message:
[14:23:46.241] [AIS] {"APIKey":"9e89c7bcda3...","BoundingBoxes":[[[-90,-180],[90,180]]]}
[14:23:46.250] [AIS] ✅ Subscription message sent!
[14:23:46.251] [AIS] 👂 Waiting for incoming messages...
[14:23:46.252] [AIS] 🔄 Receive loop started
[14:23:46.260] ✅ AIS stream started successfully!
```

---

### 4. **Receiving Messages**

#### First Message:

```
[14:23:47.123] [AIS] 📨 Message received! Size: 542 bytes
[14:23:47.124] 📡 RAW JSON RECEIVED (542 bytes)
[14:23:47.125] First 200 chars: {"Message":{"PositionReport":{"MessageID":1,"RepeatIndicator":0,"UserID":205264000,"NavigationalStatus":7,"RateOfTurn":-128,"Sog":0,"PositionAccuracy":false,"Longitude":3.566,"Latitude":51.39608...
[14:23:47.130] [AIS] 🔍 Processing message...
[14:23:47.135] [AIS] Message Type: PositionReport
[14:23:47.136] [AIS] ✅ Processing PositionReport
[14:23:47.140] [AIS] Ship: Z-8 AQUARIUS (MMSI: 205264000)
[14:23:47.141] [AIS] Position: 51.3961, 3.5660
[14:23:47.142] [AIS] ✅ Coordinates valid, emitting event
[14:23:47.145] 🚢 SHIP POSITION: MMSI=205264000, Name=Z-8 AQUARIUS, Lat=51.3961, Lon=3.5660
```

#### StandardClassB Message:

```
[14:23:48.456] [AIS] 📨 Message received! Size: 487 bytes
[14:23:48.457] 📡 RAW JSON RECEIVED (487 bytes)
[14:23:48.460] [AIS] 🔍 Processing message...
[14:23:48.462] [AIS] Message Type: StandardClassBPositionReport
[14:23:48.463] [AIS] ✅ Processing StandardClassBPositionReport
[14:23:48.465] [AIS] Ship: TRIPLE P (MMSI: 205778330)
[14:23:48.466] [AIS] ✅ Coordinates valid, emitting event
[14:23:48.470] 🚢 SHIP POSITION: MMSI=205778330, Name=TRIPLE P, Lat=51.3132, Lon=3.1210
```

#### StaticDataReport (No Position):

```
[14:23:49.789] [AIS] 📨 Message received! Size: 324 bytes
[14:23:49.790] 📡 RAW JSON RECEIVED (324 bytes)
[14:23:49.792] [AIS] 🔍 Processing message...
[14:23:49.794] [AIS] Message Type: StaticDataReport
[14:23:49.795] [AIS] ℹ️ Other message type (will show in JSON console only)
```

---

## 🔍 Troubleshooting dengan Console Logs

### ✅ **Successful Connection:**

```
[AIS] ✅ WebSocket connected successfully!
[AIS] WebSocket State: Open
🔌 CONNECTION STATUS CHANGED: CONNECTED ✅
[AIS] ✅ Subscription message sent!
[AIS] 👂 Waiting for incoming messages...
[AIS] 🔄 Receive loop started
```

**What to expect next:** Messages should start arriving within 10-30 seconds

---

### ❌ **Connection Failed:**

```
[AIS] ❌ CONNECTION FAILED!
[AIS] Exception Type: WebSocketException
[AIS] Exception Message: Unable to connect to the remote server
[AIS] Stack Trace: ...
❌ AIS ERROR: Connection failed: Unable to connect to the remote server
```

**Possible causes:**

- No internet connection
- Firewall blocking WebSocket
- API endpoint down

---

### ⚠️ **Connected but No Messages:**

```
[AIS] ✅ WebSocket connected successfully!
[AIS] ✅ Subscription message sent!
[AIS] 👂 Waiting for incoming messages...
[AIS] 🔄 Receive loop started
... (silence for 60+ seconds)
```

**Possible causes:**

- Invalid API key
- No ships in the area at the moment (rare)
- Rate limiting

---

### ⚠️ **Invalid Coordinates:**

```
[AIS] 🔍 Processing message...
[AIS] Message Type: PositionReport
[AIS] ✅ Processing PositionReport
[AIS] Ship: SHIP NAME (MMSI: 123456789)
[AIS] Position: 91.0000, 0.0000
[AIS] ⚠️ Invalid coordinates, skipping
```

**Explanation:** Ship data has invalid GPS coordinates (latitude > 90)

---

## 📊 Message Flow Timeline

### Normal Operation (First 60 seconds):

```
00:00 - Application start
00:01 - WebSocket connecting...
00:02 - WebSocket connected ✅
00:02 - Subscription sent
00:02 - Receive loop started
00:10 - First message received 📨
00:11 - Ship #1 processed 🚢
00:12 - Ship #2 processed 🚢
00:15 - Ship #3 processed 🚢
00:20 - 10 messages received
00:30 - 25 messages received
00:60 - 50+ messages received
```

---

## 🎨 Console Output Symbols

| Symbol | Meaning                    |
| ------ | -------------------------- |
| 🔌     | Connection/Network related |
| ✅     | Success                    |
| ❌     | Error/Failure              |
| ⚠️     | Warning                    |
| 📤     | Sending data               |
| 📨     | Receiving data             |
| 📡     | Raw JSON received          |
| 🚢     | Ship position processed    |
| 🔍     | Processing/Parsing         |
| 🔄     | Loop/Continuous operation  |
| 👂     | Listening/Waiting          |
| ℹ️     | Information                |

---

## 🧪 How to Run and See Console Output

### Option 1: Run from Command Line

```bash
cd C:\Users\irsad\Marinex\Marinex
dotnet run
```

**Output:** Console window will show all logs in real-time

---

### Option 2: Run from Visual Studio

1. Open project in Visual Studio
2. Press **F5** (Start Debugging) or **Ctrl+F5** (Start without Debugging)
3. **Console window will open** showing all logs
4. Dashboard window will also open

---

### Option 3: Run from VS Code

1. Open terminal in VS Code
2. Run: `dotnet run`
3. Logs appear in terminal

---

## 📝 Sample Full Output

```
=== AIS Service Initialization Started ===
[14:23:45.000] Creating AIS Service instance...
[14:23:45.010] Subscribing to events...
[14:23:45.015] Starting AIS Stream connection...
[14:23:45.020] 🔌 Attempting to start AIS stream...
[14:23:45.100] [AIS] 🔌 Connecting to WebSocket...
[14:23:45.101] [AIS] URL: wss://stream.aisstream.io/v0/stream
[14:23:46.234] [AIS] ✅ WebSocket connected successfully!
[14:23:46.235] [AIS] WebSocket State: Open
[14:23:46.236] 🔌 CONNECTION STATUS CHANGED: CONNECTED ✅
[14:23:46.240] [AIS] 📤 Sending subscription message:
[14:23:46.241] [AIS] {"APIKey":"9e89c7bcda3e91ce84e17b21190ae41ef03c44a9","BoundingBoxes":[[[-90,-180],[90,180]]]}
[14:23:46.250] [AIS] ✅ Subscription message sent!
[14:23:46.251] [AIS] 👂 Waiting for incoming messages...
[14:23:46.252] [AIS] 🔄 Receive loop started
[14:23:46.260] ✅ AIS stream started successfully!
[14:23:55.123] [AIS] 📨 Message received! Size: 542 bytes
[14:23:55.124] 📡 RAW JSON RECEIVED (542 bytes)
[14:23:55.125] First 200 chars: {"Message":{"PositionReport":{"MessageID":1,"RepeatIndicator":0,"UserID":205264000...
[14:23:55.130] [AIS] 🔍 Processing message...
[14:23:55.135] [AIS] Message Type: PositionReport
[14:23:55.136] [AIS] ✅ Processing PositionReport
[14:23:55.140] [AIS] Ship: Z-8 AQUARIUS (MMSI: 205264000)
[14:23:55.141] [AIS] Position: 51.3961, 3.5660
[14:23:55.142] [AIS] ✅ Coordinates valid, emitting event
[14:23:55.145] 🚢 SHIP POSITION: MMSI=205264000, Name=Z-8 AQUARIUS, Lat=51.3961, Lon=3.5660
[14:23:56.456] [AIS] 📨 Message received! Size: 487 bytes
[14:23:56.457] 📡 RAW JSON RECEIVED (487 bytes)
[14:23:56.460] [AIS] 🔍 Processing message...
[14:23:56.462] [AIS] Message Type: StandardClassBPositionReport
[14:23:56.463] [AIS] ✅ Processing StandardClassBPositionReport
[14:23:56.465] [AIS] Ship: TRIPLE P (MMSI: 205778330)
[14:23:56.466] [AIS] ✅ Coordinates valid, emitting event
[14:23:56.470] 🚢 SHIP POSITION: MMSI=205778330, Name=TRIPLE P, Lat=51.3132, Lon=3.1210
[14:23:57.789] [AIS] 📨 Message received! Size: 324 bytes
[14:23:57.790] 📡 RAW JSON RECEIVED (324 bytes)
[14:23:57.792] [AIS] 🔍 Processing message...
[14:23:57.794] [AIS] Message Type: StaticDataReport
[14:23:57.795] [AIS] ℹ️ Other message type (will show in JSON console only)
... (messages continue streaming)
```

---

## 🎯 What You Should See

### In Console:

- ✅ Detailed logs with timestamps
- ✅ WebSocket connection status
- ✅ Each message received
- ✅ Ship positions being processed
- ✅ Message types identified

### In Dashboard UI:

- ✅ "● Connected" status (green)
- ✅ Message counter incrementing
- ✅ Ships appearing on map
- ✅ JSON console showing raw messages
- ✅ Ship list updating

---

## ⚡ Performance Stats

After 1 minute of running, you should see approximately:

- **50-200 console log lines**
- **10-50 ships** on the map
- **30-100 JSON messages** in console
- **Message rate:** 1-3 messages per second

---

## 🐛 Debug Checklist

Before running, ensure:

- [ ] Internet connection active
- [ ] No firewall blocking port 443 (WSS)
- [ ] API key is valid (check code)
- [ ] NuGet packages restored
- [ ] Build successful (no errors)

While running, check for:

- [ ] "WebSocket connected successfully" message
- [ ] "Subscription message sent" message
- [ ] "Message received" logs appearing
- [ ] No error messages (❌)
- [ ] Ship positions being processed (🚢)

---

## 💡 Tips

1. **Redirect console to file** (for analysis):

   ```bash
   dotnet run > output.log 2>&1
   ```

2. **Filter specific logs** (PowerShell):

   ```powershell
   dotnet run | Select-String "Message received"
   ```

3. **Count messages** (PowerShell):
   ```powershell
   dotnet run | Select-String "Message received" | Measure-Object
   ```

---

## Summary

✅ **Console logs added to:**

- WebSocket connection
- Subscription sending
- Message receiving
- Message processing
- Ship position events
- Connection status changes
- All errors

✅ **Benefits:**

- See exactly what's happening
- Debug connection issues
- Verify data flow
- Monitor performance
- Track errors in real-time

**Now run `dotnet run` and watch the magic happen! 🚀**
