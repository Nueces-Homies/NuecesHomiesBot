namespace Integrations.Test;

using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using IGDBApi;
using Microsoft.Extensions.Configuration;
using ProtoBuf;
using ProtoBuf.Meta;

public class IGDBTest
{
    public class IGDBClientTest : IClassFixture<IGDBClientFixture>
    {
        private readonly IGDBClientFixture clientFixture;

        public IGDBClientTest(IGDBClientFixture clientFixture)
        {
            this.clientFixture = clientFixture;
        }

        [Fact(DisplayName = "Can fetch a game")]
        public async Task CanFetchGame()
        {
            var client = this.clientFixture.Client;
            var query = "fields *; where id = 427;";
            GameResult result = await client.QueryEndpointAsync<GameResult>("games", query);
            var game = result.Games.FirstOrDefault();

            Assert.NotNull(game);
            game.Name.Should().Be("Final Fantasy VII");

            Assert.NotNull(game.FirstReleaseDate);
            DateTime releaseDate = game.FirstReleaseDate.Value;
            releaseDate.Should().Be(new DateTime(1997, 1, 31));
        }

        [Fact(DisplayName = "Can deserialize object that uses epoch milliseconds instead of seconds")]
        public void CanDeserializeObjectWithNonSpecDateTime()
        {
            string hexString =
                "0ABC0C0804120708F0D8BAB9F0251A0308E7021A0308FF021A030880031A030881031A030882031A030883031A030884031A030885031A030887031A030888031A030889031A03088A031A03088B031A03088C031A03088D031A03088E031A03088F031A030890031A030891031A030892031A030893031A030895031A030896031A030897031A030898031A030899031A03089A031A03089B031A03089C031A03089D031A03089E031A03089F031A0308A0031A0308A1031A0308A2031A0308A3031A0308A4031A0308A5031A0308A7031A0308A9031A0308AA031A0308AB031A0308AC031A0308D10A1A0308E7121A0308E8121A030891131A0308AF2C1A0308BC391A0308E6391A0308E7391A0308E8391A0308F5391A0308F14A1A0308824B1A0308954B1A0308CF501A0308A1571A0308CE571A0308C06E1A030883731A030889731A0408DA80011A0408CB81011A0408B788011A0408948D011A0408958D011A0408968D011A04088895011A04088697011A0408FF97011A04089099011A0408999B011A0408F5A0011A0408AFA1011A0408BEA4011A0408D7A5011A0408FCA8011A040893AA011A0408F8AA011A04088BAB011A0408EEB2011A0408EFB2011A0408F0B2011A0408F1B2011A0408F3B2011A0408F4B2011A0408F5B2011A0408F6B2011A0408F7B2011A0408F8B2011A0408F9B2011A0408FAB2011A0408FBB2011A0408FCB2011A0408FDB2011A0408E8B3011A0408E1B5011A0408CABB011A0408E0BD011A0408D0BE011A040881D0011A040898D2011A040892D6011A0408E2DA011A0408BFF6011A0408DF9F021A040886A1021A0408DFA1021A0408C9A2021A0408ABA3021A0408DCAC021A0408DDAC021A0408DCC0021A0408E6C6021A0408E7C6021A0408EAC6021A0408EBC6021A0408ECC6021A0408EDC6021A0408EEC6021A0408EFC6021A0408F0C6021A0408F1C6021A0408F2C6021A0408F3C6021A0408F5C6021A0408F8C6021A0408FBC6021A0408FCC6021A0408FDC6021A0408FEC6021A0408FFC6021A040880C7021A04089783031A0408CC92031A040896AA031A0408FAAD031A0408FBAD031A040889AE031A0408C1C3031A0408B89C041A0408A0CF041A0408A1CF041A040880D3041A040883D3041A04088ED5041A0408B2DB041A0408A1F1041A0408EDF7041A0408B6FA041A0408CE91051A04088AB7051A0408A6BE051A0408F5BF051A0408C3C3051A0408DE96061A0408C3C2061A0408FCCC061A0408DCD5061A040895D7061A040899D7061A04089CEE061A0408F5F1061A0408E0F7061A0408CEA4071A040898A6071A0408D8B6071A0408F5B6071A04088FD5071A0408D8E6071A040884E7071A040887E7071A040885F1071A04089B87081A0408F490081A0408CA95081A0408CE95081A0408D298081A0408A0A8081A0408AAAF081A0408C0D1081A0408DADE081A040880DF081A04088EDF081A040898E5081A0408A5E5081A0408A6E5081A0408A8E5081A0408B1EA081A040899F3081A0408E6F4081A040895F5081A0408FAF5081A0408A6F6081A0408AAF6081A0408FCF7081A0408B9F9081A0408BCF9081A0408D6FA081A0408A4FB081A0408C3FC081A0408F5FC081A04088EFD081A0408BAA5091A040884DA091A040885DA091A040886DA091A040887DA091A040888DA091A040889DA091A040895DC091A0408C0DC091A040892F10A1A040893F10A1A040886B20B1A040884B60C1A040892B60C1A040891C30C1A0408E9C60C1A040887C70C1A0408ADD10C1A0408B2D10C1A0408B4D10C1A040896FE0C1A0408DA840D1A0408CA8F0D1A0408D4A30D1A0408BEA50D1A0408D2C20D1A0408D3C20D1A0408D5C20D1A0408D6C20D1A0408D8C20D1A0408D9C20D1A0408DAC20D1A0408DDC20D1A0408E0C20D1A0408E1C20D1A0408E2C20D1A0408E3C20D1A0408E4C20D1A0408E5C20D1A0408E6C20D1A0408E7C20D1A0408E8C20D1A0408E9C20D1A0408EAC20D1A0408EBC20D1A0408ECC20D1A0408EDC20D1A0408EDCB0D1A0408DBF20D1A040896FC0D220D46696E616C2046616E746173792A0D66696E616C2D66616E746173793206089BC68F9D063A2D68747470733A2F2F7777772E696764622E636F6D2F6672616E6368697365732F66696E616C2D66616E74617379422432626465636431302D653162302D326164612D333336622D316433376163336363313963";

            byte[] data = Convert.FromHexString(hexString);

            using var stream = new MemoryStream(data);
            RuntimeTypeModel.Default.IncludeDateTimeKind = true;
            var result = Serializer.Deserialize<FranchiseResult>(stream);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Franchises);

            Franchise first = result.Franchises.First();
            Assert.NotNull(first);

            DateTime? createdDate = first.CreatedAt;
            createdDate.Should().BeCloseTo(new DateTime(2011, 3, 30), TimeSpan.FromDays(1));
        }

        public static IEnumerable<object[]> GetTypeValidationTestData()
        {
            return Assembly
                .GetAssembly(typeof(IGDBTimestamp))?
                .GetExportedTypes()
                .Where(t => t.IsAssignableTo(typeof(IExtensible)))
                .Select(t => new object[] { t })!;
        }

        [Theory(DisplayName = "IGDB Model should not contain any DateTime fields")]
        [MemberData(nameof(IGDBClientTest.GetTypeValidationTestData))]
        public void ModelTypeDoesNotContainDateTimeField(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                property.PropertyType.Should().NotBeAssignableTo(typeof(DateTime?),
                    "because IGDB protobuf models should be using IGDBTimestamp instead of DateTime fields");
            }
        }

        [Fact(DisplayName = "Malformed query should throw an exception")]
        public async void BadQueryExceptionThrownForBadQuery()
        {
            var client = this.clientFixture.Client;
            
            // Ending semicolon missing
            string query = "fields *; where id = 10";

            var action = async () => await client.QueryEndpointAsync<GameResult>("games", query);
            await action.Should().ThrowExactlyAsync<IGDBBadQueryException>();
        }
        
        [Fact(DisplayName = "Invalid rquest should throw an exception")]
        public async void BadRequestExceptionShouldBeThrownForNotFound()
        {
            var client = this.clientFixture.Client;
            
            // Ending semicolon missing
            string query = "fields *; where id = 10;";

            var action = async () => await client.QueryEndpointAsync<GameResult>("bad_games", query);
            await action.Should().ThrowExactlyAsync<IGDBBadRequestException>();
        }
    }

    public class IGDBWrapperTests : IClassFixture<IGDBFixture>
    {
        private readonly IGDBFixture clientFixture;

        public IGDBWrapperTests(IGDBFixture clientFixture)
        {
            this.clientFixture = clientFixture;
        }

        [Fact]
        public async Task GameSearchContainsExpectedFields()
        {
            var client = this.clientFixture.Client;
            var games = await client.SearchForGamesAsync("Halo");
            
            using var _ = new AssertionScope();
            
            Assert.NotNull(games);
            Assert.NotEmpty(games);
            foreach (var game in games)
            {
                game.Id.Should().BePositive();
                game.Name.Should().NotBeNull();
                game.Name.Should().Contain("Halo");
                
                Assert.NotNull(game.ReleaseDates);
                Assert.NotEmpty(game.ReleaseDates);
                foreach (var releaseDate in game.ReleaseDates)
                {
                    releaseDate.Human.Should().NotBeNullOrEmpty();
                    releaseDate.Category.Should().BeDefined();
                }
                
                Assert.NotNull(game.InvolvedCompanies);
                Assert.NotEmpty(game.InvolvedCompanies);
                foreach (var involvedCompany in game.InvolvedCompanies)
                {
                    Assert.NotNull(involvedCompany.Company);
                    involvedCompany.Company.Name.Should().NotBeNullOrEmpty();
                }
            }
        }

        [Fact]
        public async Task GameLookupContainsExpectedFields()
        {
            var client = this.clientFixture.Client;
            var game = await client.GetGameAsync(152242);

            Assert.NotNull(game);
            using (new AssertionScope())
            {
                game.Id.Should().Be(152242);
                game.Name.Should().Be("A Plague Tale: Requiem");
                game.Url.Should().Be("https://www.igdb.com/games/a-plague-tale-requiem");
                game.Summary.Should().NotBeEmpty();
                game.AggregatedRating.Should().BeGreaterThan(0);

                var firstReleaseDate = game.FirstReleaseDate;
                firstReleaseDate.Should().NotBeNull();
                firstReleaseDate.Value.AsDateTime().Should().Be(new DateTime(2022, 10, 18));

                game.Genres.Should().NotBeEmpty();
                game.Themes.Should().NotBeEmpty();
                game.PlayerPerspectives.Should().NotBeEmpty();
                game.GameModes.Should().NotBeEmpty();
                game.Platforms.Should().NotBeEmpty();
                game.InvolvedCompanies.Should().NotBeEmpty();

                foreach (var involvedCompany in game.InvolvedCompanies)
                {
                    involvedCompany.Company.Should().NotBeNull();
                }
                
                game.ReleaseDates.Should().NotBeEmpty();

                var releaseDate = game.ReleaseDates.First();
                releaseDate.Human.Should().Be("Oct 18, 2022");
                releaseDate.Category.Should().Be(DateFormatChangeDateCategoryEnum.Yyyymmmmdd);

                Assert.NotNull(releaseDate.Date);
                releaseDate.Date.Value.AsDateTime().Should().Be(new DateTime(2022, 10, 18));
            }
        }

        [Fact]
        public async Task CanLookupGamesInBulk()
        {
            var ids = new HashSet<ulong>();
            ids.Add(152242);
            ids.Add(427);
            
            var client = this.clientFixture.Client;
            var games = await client.GetGamesAsync(ids);
            
            Assert.NotNull(games);
            games.Should().NotBeEmpty();

            using var _ = new AssertionScope();

            foreach (var game in games)
            {
                ids.Should().Contain(game.Id);
                game.Name.Should().NotBeNullOrEmpty();
                game.ReleaseDates.Should().NotBeNullOrEmpty();

                foreach (var date in game.ReleaseDates)
                {
                    date.Human.Should().NotBeNullOrEmpty();
                    Assert.NotNull(date.Platform);
                    date.Platform.Id.Should().BePositive();
                }
            }
        }
        
    }

    public class IGDBClientFixture
    {
        public IGDBClient Client { get; }

        public IGDBClientFixture()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();

            var (clientId, clientSecret) = config.GetTwitchApiKeys();
            Client = new IGDBClient(clientId, clientSecret);
        }
    }

    public class IGDBFixture
    {
        public IGDB Client { get; }

        public IGDBFixture()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .Build();

            var (clientId, clientSecret) = config.GetTwitchApiKeys();
            Client = new IGDB(clientId, clientSecret);
        }
    }
}

file static class ConfigurationExtensions
{
    public static (string, string) GetTwitchApiKeys(this IConfiguration config)
    {
        var clientId = config["TWITCH_CLIENT_ID"] ?? throw new Exception("TWITCH_CLIENT_ID missing");
        var clientSecret = config["TWITCH_CLIENT_SECRET"] ?? throw new Exception("TWITCH_CLIENT_SECRET missing");
        return (clientId, clientSecret);
    }
}