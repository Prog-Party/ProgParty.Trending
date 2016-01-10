namespace ProgParty.Trending.Api.Parameter
{
    public class OverviewParameter
    {
        public int Paging { get; set; }

        public bool StartOver { get; set; } = false;

        public ArticleCategory Category { get; set; } = ArticleCategory.All;
    }

    public enum ArticleCategory
    {
        Advertising = 1
        ,All = 0
        ,Animals = 2
        ,Architecture = 3
        ,Art = 4
        ,Automotive = 5
        ,BodyArt = 6
        ,Comics = 7
        ,DigitalArt = 8
        ,DIY = 9
        ,Drawing = 10
        ,Entertainment = 11
        ,FoodArt = 12
        ,Funny = 13
        ,FurnitureDesign = 14
        ,GoodNews = 15
        ,GraphicDesign = 16
        ,History = 17
        ,Home = 18
        ,Illustration = 19
        ,Installation = 20
        ,InteriorDesign = 21
        ,LandArt = 22
        ,Nature = 23
        ,NeedleAndThread = 24
        ,OpticalIllusions = 25
        ,Other = 26
        ,Packaging = 27
        ,Painting = 28
        ,PaperArt = 29
        ,Parenting = 30
        ,Photography = 31
        ,Pics = 32
        ,ProductDesign = 33
        ,Recycling = 34
        ,Science = 35
        ,Sculpting = 36
        ,SocialIssues = 37
        ,StreetArt = 38
        ,Style = 39
        ,Technology = 40
        ,Travel = 41
        ,Typography = 42
        ,Video = 43
        ,Weird = 44
    }
}
