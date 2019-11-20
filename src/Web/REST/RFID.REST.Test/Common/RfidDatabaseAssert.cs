namespace RFID.REST.Test.Common
{
    using RFID.REST.Areas.Administration.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    internal class RfidDatabaseAssert
    {
        private static readonly Dictionary<Type, Func<Task<int>>> typeToCntFuncMap = new Dictionary<Type, Func<Task<int>>>
        {
            [typeof(RegisterAdministrationUserRequestModel)] = RfidDatabase.GetAdministrationUsersCountAsync,
            [typeof(RegisterTagRequestModel)] = RfidDatabase.GetAccessControlTagsCountAsync,
            [typeof(RegisterTagUserRequestModel)] = RfidDatabase.GetAccessControlUsersCountAsync,
            [typeof(RegisterAccessPointRequestModel)] = RfidDatabase.GetAccessPointsCountAsync,
            [typeof(UnKknownAccessPointMock)] = RfidDatabase.GetUnKnownAccessPointCountsAsync,
            [typeof(UnKnownTagMock)] = RfidDatabase.GetUnknownTagCntAsync
        };

        public static async Task<RfidDatabaseAssert> CreateAsync()
        {
            var obj = new RfidDatabaseAssert();
            await obj.InitAsync();

            return obj;
        }

        private Dictionary<Type, int> cntMap = new Dictionary<Type, int>();

        private RfidDatabaseAssert() { }

        private async Task InitAsync()
        {
            this.cntMap = await GetDatabaseCntsAsync();
        }

        public async Task AssertCntAsync(params Object[] objs)
        {
            var actual = await GetDatabaseCntsAsync();

            var flatten = new List<Object>(objs);
            var uniqueAccessControlUsers = GetTagUsersRequestModels(objs);
            flatten.AddRange(uniqueAccessControlUsers);

            var typeGroups = flatten
                .Where(x => IsUpdateType(x) == false)
                .GroupBy(x => x.GetType())
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var (type, cnt) in this.cntMap)
            {
                var actualCnt = actual[type];
                var expectedCnt = cnt;
                if (typeGroups.TryGetValue(type, out var requestModels))
                {
                    expectedCnt = expectedCnt + requestModels.Count;
                }

                Assert.Equal(expected: expectedCnt, actual: actualCnt);
            }

            var unknownTypes = typeGroups.Select(x => x.Key).Except(typeToCntFuncMap.Keys);
            if (unknownTypes.Any())
            {
                throw new ArgumentException("Missing cnt type");
            }
        }

        public async Task AssertStateAsync<T>(String tableName, int id, T expectedState)
        {
            var expectedMap = (from p in typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                               select (name: p.Name, value: p.GetValue(expectedState))
                              ).ToDictionary(x => x.name, x => x.value);

            var actualMap = await RfidDatabase.GetRecordStateByTableNameAsync(tableName, id);
            actualMap = actualMap.Where(x => expectedMap.Keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            Assert.True(expectedMap.OrderBy(x => x.Key).SequenceEqual(actualMap.OrderBy(x => x.Key)));
        }

        private static async Task<Dictionary<Type, int>> GetDatabaseCntsAsync()
        {
            var tasks = new List<(Type type, Task<int> task)>();

            foreach (var (type, func) in typeToCntFuncMap)
            {
                var task = func();
                tasks.Add((type, task));
            }

            await Task.WhenAll(tasks.Select(x => x.task).ToList());

            return tasks.ToDictionary(x => x.type, x => x.task.Result);
        }

        private static IReadOnlyCollection<RegisterTagUserRequestModel> GetTagUsersRequestModels(IEnumerable<Object> objs)
        {
            var result = new List<RegisterTagUserRequestModel>();

            foreach (var obj in objs)
            {
                if (obj is RegisterTagRequestModel rm)
                {
                    result.Add(new RegisterTagUserRequestModel { Name = rm.UserName });
                }
                else if (obj is UpdateTagRequestModel urm)
                {
                    result.Add(new RegisterTagUserRequestModel
                    {
                        Name = urm.UserName
                    });
                }
            }

            return result.GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }

        private static bool IsUpdateType(Object obj)
        {
            return new[] { typeof(UpdateTagRequestModel) }.Contains(obj.GetType());
        }
    }
}
