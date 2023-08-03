namespace IwbrDaemon.Trading
{
    public enum eAnalysAlgorithm {
        Null = 0
        ,SMA1h = 1
        ,CryptoHopper = 2
        ,SMA2h = 3
    }


    public enum eAnaylysOrderResult {
        Base = 10
        //-----------
        ,High = 10
        ,Medium = 15
        ,Low = 20
    }

    public enum ePositionType {
        Short = -1
        ,Long =1
    }

    public enum ePositionAction {
        Open = -1
        ,Nothing = 0
        ,Close =1
        ,Skip = 10
    }

}