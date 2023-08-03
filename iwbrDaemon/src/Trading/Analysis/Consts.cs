namespace IwbrDaemon.Trading.Analysis
{
    public enum eConditions {
        Overbought = 1
        ,Neutral = 0 
        ,Oversold = -1
    }

    public enum eTrendStrenght{
        NoStability = 0
        ,Strong = 1
        ,VeryStrong = 2
        ,ExtremelyStrong = 3
    }

    public enum eActions{
        Sell = -1
        ,Neutral = 0 
        ,Buy = 1        
    }

}