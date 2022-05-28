namespace DominoTrain.Enumerations
{
    public static class CasinoBackgroundsMap
    {
        public static Dictionary<CasinoBackgroundType, (string filename, string description)> BackgroundTypeMap
    => new Dictionary<CasinoBackgroundType, (string filename, string description)> {
            {CasinoBackgroundType.WoodPanels, (filename: "casino_bg_0", description: "Wood Panels")},
            {CasinoBackgroundType.RedCardSuits, (filename: "casino_bg_1", description: "Red Card Suits")},
            {CasinoBackgroundType.GreenPattern, (filename: "casino_bg_2", description: "Green Pattern")},
            {CasinoBackgroundType.GreenSnowflakes, (filename: "casino_bg_3", description: "Green Snowflakes")},
            {CasinoBackgroundType.DarkGreenFelt, (filename: "casino_bg_4", description: "Dark Green Felt")},
            {CasinoBackgroundType.GreenFelt, (filename: "casino_bg_5", description: "Green Felt")},
            {CasinoBackgroundType.GreenVinyl, (filename: "casino_bg_6", description: "Green Vinyl")},
            {CasinoBackgroundType.GreenMarble, (filename: "casino_bg_7", description: "Green Marble")},
            {CasinoBackgroundType.RedDiamonds, (filename: "casino_bg_8", description: "Red Diamonds")}
        };

        public static (string filename, string description) ToTuple(this CasinoBackgroundType backgroundType)
        {
            if (!BackgroundTypeMap.ContainsKey(backgroundType))
            {
                throw new KeyNotFoundException(message: backgroundType.ToString());
            }
            return BackgroundTypeMap[backgroundType];
        }

        public static string ToFilename(this CasinoBackgroundType backgroundType)
        {
            return backgroundType.ToTuple().filename;
        }

        public static string ToDescription(this CasinoBackgroundType backgroundType)
        {
            return backgroundType.ToTuple().description;
        }
    }
}
