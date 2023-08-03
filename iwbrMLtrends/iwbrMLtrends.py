import json
from Clients.IwbrDb import IwbrDb
from config import Config



with open('config.json', 'r') as file:
    json_data = json.load(file)

config = Config(json_data)
iwbrDb = IwbrDb(config)



symbol = "ADABUSD"
interval = "30m"
last_klines = iwbrDb.get_last_klines(symbol, interval, 2000)[:-1]











import datetime
import decimal
import numpy as np
import pandas as pd
import tensorflow as tf
from sklearn.preprocessing import MinMaxScaler
from sklearn.model_selection import TimeSeriesSplit
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from Types.Exchange.KLine import KLineReduced

# Assuming you have the list of KLine instances named last_klines
# Convert last_klines to a pandas DataFrame
data = {
    "symbol": [kline.symbol for kline in last_klines],
    "interval": [kline.interval for kline in last_klines],
    "openTime": [kline.openTime for kline in last_klines],
    "openPrice": [float(kline.openPrice) for kline in last_klines],
    "highPrice": [float(kline.highPrice) for kline in last_klines],
    "lowPrice": [float(kline.lowPrice) for kline in last_klines],
    "closePrice": [float(kline.closePrice) for kline in last_klines],
    "volume": [float(kline.volume) for kline in last_klines],
    "closeTime": [kline.closeTime for kline in last_klines],
}



df = pd.DataFrame(data)

# Data Preprocessing
# Drop unnecessary columns and sort by lastUpdateTime
df = df.sort_values(by="closeTime")
df = df.drop(columns=["symbol", "interval", "openTime", "closeTime"])

# Feature Scaling
scaler = MinMaxScaler(feature_range=(0, 1))
scaled_data = scaler.fit_transform(df)

# Prepare Input and Target Data
X, y = [], []
time_steps = 4  # len(df) - 1  # Number of time steps for input sequence
print("time_steps: ", time_steps)


for i in range(len(scaled_data) - time_steps):
    X.append(scaled_data[i : i + time_steps])
    y.append(scaled_data[i + time_steps])

X = np.array(X)
y = np.array(y)

# Use TimeSeriesSplit for Train-Test Split
tscv = TimeSeriesSplit(n_splits=5)  # You can change the number of splits as needed

for train_index, test_index in tscv.split(X):
    X_train, X_test = X[train_index], X[test_index]
    y_train, y_test = y[train_index], y[test_index]

    # Build the LSTM Model
    model = Sequential()
    model.add(LSTM(64, input_shape=(X_train.shape[1], X_train.shape[2])))
    model.add(Dense(X_train.shape[2]))  # Number of features in the target (8 in this case)

    model.compile(loss="mean_squared_error", optimizer="adam")
    model.summary()

    # Train the Model
    model.fit(X_train, y_train, epochs=1000, batch_size=256)

    # Make Predictions
    predicted_scaled_values = model.predict(X_test)

    # Inverse Transform to Get Actual Predictions
    predicted_values = scaler.inverse_transform(predicted_scaled_values)

    # Create Instances of KLineReduced
    predicted_klines = []
    last_close_time = last_klines[-1].closeTime

    for prediction in predicted_values:
        symbol = "ADABUSD"  # Replace with the desired symbol for prediction
        interval = "30m"    # Replace with the desired interval for prediction
        openPrice, highPrice, lowPrice, closePrice, volume = prediction

        openTime = last_close_time + 1
        closeTime = last_close_time + 30*60*1000

        # Create the KLineReduced instance with the predicted values for the next time unit
        kline_reduced = KLineReduced(
            symbol,
            interval,
            openTime,
            float(openPrice),
            float(highPrice),
            float(lowPrice),
            float(closePrice),
            float(volume),
            closeTime
        )
        predicted_klines.append(kline_reduced)

        # Update last_close_time for the next prediction
        last_close_time = closeTime

    # Now predicted_klines contains the instances of KLineReduced with the predicted values for the next 8 time units


    # Assuming you have the list of KLineReduced instances named predicted_klines
    for kline in predicted_klines[:16]:
        print("Symbol:", kline.symbol)
        print("Interval:", kline.interval)
        print("Open Time:", datetime.datetime.fromtimestamp(kline.openTime / 1000.0).strftime('%Y-%m-%d %H:%M:%S.%f'))
        #print("Open Price:", kline.openPrice)
        print("High Price:", kline.highPrice)
        print("Low Price:", kline.lowPrice)
        #print("Close Price:", kline.closePrice)
        print("Volume:", kline.volume)
        print("Close Time:", datetime.datetime.fromtimestamp(kline.closeTime / 1000.0).strftime('%Y-%m-%d %H:%M:%S.%f'))
        print("----------------------------------")
