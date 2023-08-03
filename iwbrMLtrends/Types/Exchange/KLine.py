import datetime
import decimal

class KLineReduced:
    def __init__(self, symbol, interval, openTime, openPrice, highPrice, lowPrice, closePrice, volume, closeTime):
        self.symbol = symbol
        self.interval = interval
        self.openTime = openTime
        self.openPrice = decimal.Decimal(openPrice)
        self.highPrice = decimal.Decimal(highPrice)
        self.lowPrice = decimal.Decimal(lowPrice)
        self.closePrice = decimal.Decimal(closePrice)
        self.volume = decimal.Decimal(volume)
        self.closeTime = closeTime


class KLine:
    def __init__(self, lastUpdateTime, symbol, interval, openTime, openPrice, highPrice, lowPrice, closePrice, volume, closeTime, quoteVolume, tradeCount, takerBuyBaseVolume, takerBuyQuoteVolume, isKLineClosed, source):
        self.lastUpdateTime = lastUpdateTime
        self.symbol = symbol
        self.interval = interval
        self.openTime = openTime
        self.openPrice = decimal.Decimal(openPrice)
        self.highPrice = decimal.Decimal(highPrice)
        self.lowPrice = decimal.Decimal(lowPrice)
        self.closePrice = decimal.Decimal(closePrice)
        self.volume = decimal.Decimal(volume)
        self.closeTime = closeTime
        self.quoteVolume = decimal.Decimal(quoteVolume)
        self.tradeCount = int(tradeCount)
        self.takerBuyBaseVolume = decimal.Decimal(takerBuyBaseVolume)
        self.takerBuyQuoteVolume = decimal.Decimal(takerBuyQuoteVolume)
        self.isKLineClosed = bool(isKLineClosed)
        self.source = source
