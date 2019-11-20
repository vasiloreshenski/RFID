namespace RFID.REST.Test.Administration
{
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;
    using RFID.REST.Test.Common;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("Integration")]
    public class TagsControllerTests
    {
        public TagsControllerTests()
        {
            RfidDockerHttpClient.RestMssqlAsync().Wait();
        }

        #region Register

        [Fact]
        public async Task Register_When_New_Tag()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();

                using (var tagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM);
        }

        [Fact]
        public async Task Register_When_User_Already_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var ftagRM = Examples.Tag();
            var stagRM = Examples.Tag(Guid.NewGuid(), "test");

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();

                using (var fhttpResponse = await RfidHttpClient.RegisterTagAsync(ftagRM, token))
                using (var shttpResponse = await RfidHttpClient.RegisterTagAsync(stagRM, token))
                {
                    RfidAssert.AssertHttpResponse(fhttpResponse, System.Net.HttpStatusCode.OK);
                    RfidAssert.AssertHttpResponse(shttpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, ftagRM, stagRM);
        }

        [Fact]
        public async Task Register_When_Tag_Already_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();
                using (var fhttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                using (var shttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(fhttpResponse, System.Net.HttpStatusCode.OK);
                    await RfidAssert.AssertHttpResponseAsync(shttpResponse, System.Net.HttpStatusCode.BadRequest, (false, CommandStatus.Dublicate));
                }
            }
        }

        [Fact]
        public async Task Register_When_UnAuthorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var tagRM = Examples.Tag();

            using (var httpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, null))
            {
                RfidAssert.AssertHttpResponse(httpResponse, System.Net.HttpStatusCode.Unauthorized);
            }

            await assertDatabase.AssertCntAsync();
        }

        [Theory]
        [InlineData("", 2, "username")]
        [InlineData(null, 2, "username")]
        [InlineData("1", -1, "username")]
        [InlineData("1", 3, "")]
        [InlineData("1", 3, null)]
        [InlineData(null, 3, "")]
        [InlineData(null, 3, null)]
        [InlineData(null, -1, null)]
        public async Task Register_When_Invalid(String number, int accessLevel, String userName)
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag(number, accessLevel, userName);

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();
                using (var tagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.BadRequest);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        #endregion Register


        #region Activate

        [Fact]
        public async Task Activate_When_Tag_Not_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(tagRM.Number);

                    using (var activateHttpResponse = await RfidHttpClient.ActivateTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = true });
        }

        [Fact]
        public async Task Activate_When_Tag_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var activateHttpResponse = await RfidHttpClient.ActivateTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = true });
        }

        [Fact]
        public async Task Activate_When_One_Active_And_One_Not_Active_Tag()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var ftagRM = Examples.Tag();
            var stagRM = Examples.Tag(Guid.NewGuid(), "test");

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var fregisterTagHttpResponse = await RfidHttpClient.RegisterTagAsync(ftagRM, token))
                using (var sregisterTagHttpResponse = await RfidHttpClient.RegisterTagAsync(stagRM, token))
                {
                    RfidAssert.AssertHttpResponse(fregisterTagHttpResponse, System.Net.HttpStatusCode.OK);
                    RfidAssert.AssertHttpResponse(sregisterTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(ftagRM.Number);

                    using (var factivateHttpResponse = await RfidHttpClient.ActivateTagAsync(ftagRM.Number, token))
                    using (var sactivateHttpResponse = await RfidHttpClient.ActivateTagAsync(stagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(factivateHttpResponse, System.Net.HttpStatusCode.OK);
                        RfidAssert.AssertHttpResponse(sactivateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var ftagId = await RfidDatabase.GetTagIdByNumberAsync(ftagRM.Number);
            var stagId = await RfidDatabase.GetTagIdByNumberAsync(stagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, ftagRM, stagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", ftagId, new { Id = ftagId, IsActive = true });
            await assertDatabase.AssertStateAsync("access_control.Tags", stagId, new { Id = stagId, IsActive = true });
        }

        [Fact]
        public async Task Acitvate_When_Tag_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var activateHttpResponse = await RfidHttpClient.ActivateTagAsync("unknown", token))
                {
                    RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        [Fact]
        public async Task Activate_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(tagRM.Number);

                    using (var activateHttpResponse = await RfidHttpClient.ActivateTagAsync(tagRM.Number, null))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("[access_control].[Tags]", tagId, new { Id = tagId, IsActive = false });
        }

        #endregion Activate


        #region DeActivate

        [Fact]
        public async Task DeActivate_When_Tag_Not_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(tagRM.Number);

                    using (var activateHttpResponse = await RfidHttpClient.DeActivateTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = false });
        }

        [Fact]
        public async Task DeActivate_When_Tag_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var activateHttpResponse = await RfidHttpClient.DeActivateTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = false });
        }

        [Fact]
        public async Task DeActivate_When_One_Active_And_One_Not_Active_Tag()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var ftagRM = Examples.Tag();
            var stagRM = Examples.Tag(Guid.NewGuid(), "test");

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var fregisterTagHttpResponse = await RfidHttpClient.RegisterTagAsync(ftagRM, token))
                using (var sregisterTagHttpResponse = await RfidHttpClient.RegisterTagAsync(stagRM, token))
                {
                    RfidAssert.AssertHttpResponse(fregisterTagHttpResponse, System.Net.HttpStatusCode.OK);
                    RfidAssert.AssertHttpResponse(sregisterTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var sactivateHttpResponse = await RfidHttpClient.DeActivateTagAsync(stagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(sactivateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var ftagId = await RfidDatabase.GetTagIdByNumberAsync(ftagRM.Number);
            var stagId = await RfidDatabase.GetTagIdByNumberAsync(stagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, ftagRM, stagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", ftagId, new { Id = ftagId, IsActive = true });
            await assertDatabase.AssertStateAsync("access_control.Tags", stagId, new { Id = stagId, IsActive = false });
        }

        [Fact]
        public async Task DeActivate_When_Tag_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var activateHttpResponse = await RfidHttpClient.DeActivateTagAsync("unknown", token))
                {
                    RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        [Fact]
        public async Task DeActivate_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var activateHttpResponse = await RfidHttpClient.DeActivateTagAsync(tagRM.Number, null))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("[access_control].[Tags]", tagId, new { Id = tagId, IsActive = true });
        }

        #endregion DeActivate


        #region DeActivate

        [Fact]
        public async Task Delete_When_Tag_Not_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(tagRM.Number);

                    using (var activateHttpResponse = await RfidHttpClient.DeleteTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsDeleted = true, IsActive = false });
        }

        [Fact]
        public async Task Delete_When_Tag_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var activateHttpResponse = await RfidHttpClient.DeleteTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsDeleted = true, IsActive = true });
        }

        [Fact]
        public async Task Delete_When_Tag_Already_Deleted()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var factivateHttpResponse = await RfidHttpClient.DeleteTagAsync(tagRM.Number, token))
                    using (var sactivateHttpResponse = await RfidHttpClient.DeleteTagAsync(tagRM.Number, token))
                    {
                        RfidAssert.AssertHttpResponse(factivateHttpResponse, System.Net.HttpStatusCode.OK);
                        RfidAssert.AssertHttpResponse(sactivateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsDeleted = true, IsActive = true });
        }

        [Fact]
        public async Task Delete_When_Tag_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var activateHttpResponse = await RfidHttpClient.DeleteTagAsync("unknown", token))
                {
                    RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        [Fact]
        public async Task Delete_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    using (var activateHttpResponse = await RfidHttpClient.DeleteTagAsync(tagRM.Number, null))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            var tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("[access_control].[Tags]", tagId, new { Id = tagId, IsActive = true, IsDeleted = false });
        }

        #endregion DeActivate

        #region Access Level

        [Fact]
        public async Task Change_Access_Level_When_Tag_Not_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    await RfidDatabase.DeActivateTagAsync(tagRM.Number);

                    tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);
                    using (var activateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = tagId, AccessLevel = AccessLevel.Low }, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = false, LevelId = (int)AccessLevel.Low });
        }

        [Fact]
        public async Task DeletChange_Access_Level_When_Tag_Active()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);
                    using (var activateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = tagId, AccessLevel = AccessLevel.High }, token))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = true, LevelId = (int)AccessLevel.High });
        }

        [Fact]
        public async Task Change_Access_Level_When_Tag_Already_Has_The_Same_Access_level()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);
                    using (var factivateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = tagId, AccessLevel = AccessLevel.Low }, token))
                    using (var sactivateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = tagId, AccessLevel = AccessLevel.Low }, token))
                    {
                        RfidAssert.AssertHttpResponse(factivateHttpResponse, System.Net.HttpStatusCode.OK);
                        RfidAssert.AssertHttpResponse(sactivateHttpResponse, System.Net.HttpStatusCode.OK);
                    }
                }
            }

            tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { Id = tagId, IsActive = true, LevelId = (int)AccessLevel.Low });
        }

        [Fact]
        public async Task Change_Access_Level_When_Tag_Does_Not_Exists()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var activateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = 0, AccessLevel = AccessLevel.High }, token))
                {
                    RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.NotFound);
                }
            }

            await assertDatabase.AssertCntAsync(userRM);
        }

        [Fact]
        public async Task Change_Access_Level_When_Not_Authorized()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();
            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var authHttpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(authHttpResponse);
                var token = await authToken.GetTokenAsync();

                using (var registerTagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(registerTagHttpResponse, System.Net.HttpStatusCode.OK);

                    tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);
                    using (var activateHttpResponse = await RfidHttpClient.ChangeTagAccessLevelAsync(new ChangeTagAccessLevelRequestModel { Id = tagId, AccessLevel = AccessLevel.High }, null))
                    {
                        RfidAssert.AssertHttpResponse(activateHttpResponse, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }
            
            await assertDatabase.AssertCntAsync(userRM, tagRM);
            await assertDatabase.AssertStateAsync("[access_control].[Tags]", tagId, new { Id = tagId, IsActive = true, LevelId = (int)AccessLevel.Mid });
        }

        #endregion Access Level

        #region Update

        [Fact]
        public async Task Update_When_User_Changed()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;
            var tagNewUsername = "new username";
            var tagUpdateRM = (UpdateTagRequestModel)null;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();

                using (var tagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.OK);
                }

                tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

                tagUpdateRM = Examples.TagUpdate(tagId, tagNewUsername);
                using (var tagHttpResponse = await RfidHttpClient.UpdateTagAsync(tagUpdateRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, tagUpdateRM);
            var expectedUserId = await RfidDatabase.GetTagUserIdByUserNameAsync(tagNewUsername);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { UserId = expectedUserId });
        }

        [Fact]
        public async Task Update_When_User_Is_The_Same()
        {
            var assertDatabase = await RfidDatabaseAssert.CreateAsync();

            var userRM = Examples.Administrator();
            var tagRM = Examples.Tag();
            var tagId = 0;
            var tagUpdateRM = (UpdateTagRequestModel)null;

            await RfidHttpClient.RegisterUserAsync(userRM);

            using (var httpResponse = await RfidHttpClient.GenerateAuthTokenAsync(userRM))
            {
                var authToken = await AuthTokenHelper.FromHttpResponseMessageAsync(httpResponse);
                var token = await authToken.GetTokenAsync();

                using (var tagHttpResponse = await RfidHttpClient.RegisterTagAsync(tagRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.OK);
                }

                tagId = await RfidDatabase.GetTagIdByNumberAsync(tagRM.Number);

                tagUpdateRM = Examples.TagUpdate(tagId, tagRM.UserName);
                using (var tagHttpResponse = await RfidHttpClient.UpdateTagAsync(tagUpdateRM, token))
                {
                    RfidAssert.AssertHttpResponse(tagHttpResponse, System.Net.HttpStatusCode.OK);
                }
            }

            await assertDatabase.AssertCntAsync(userRM, tagRM, tagUpdateRM);
            var expectedUserId = await RfidDatabase.GetTagUserIdByUserNameAsync(tagRM.UserName);
            await assertDatabase.AssertStateAsync("access_control.Tags", tagId, new { UserId = expectedUserId });
        }

        #endregion Update
    }
}
