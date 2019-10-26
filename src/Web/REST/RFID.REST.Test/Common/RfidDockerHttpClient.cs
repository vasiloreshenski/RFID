namespace RFID.REST.Test.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    public static class RfidDockerHttpClient
    {
        public static async Task RestMssqlAsync()
        {
            var containerName = "rfid-mssql";
            var imageName = "rfid-mssql";

            using (var client = new DockerClientConfiguration(new Uri("http://192.168.0.105:2375")).CreateClient())
            {
                var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true });
                var mssqlContainer = containers.SingleOrDefault(x => x.Names.Contains($"/{containerName}"));
                if (mssqlContainer != null)
                {
                    await client.Containers.StopContainerAsync(containerName, new ContainerStopParameters());
                    await client.Containers.RemoveContainerAsync(containerName, new ContainerRemoveParameters());
                }

                await client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Name = containerName,
                    Image = imageName,
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<String, IList<PortBinding>>
                        {
                            ["1433/tcp"] = new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostIP = "0.0.0.0",
                                    HostPort = "1433/tcp"
                                }
                            }
                        }
                    },
                    NetworkingConfig = new NetworkingConfig
                    {
                        EndpointsConfig = new Dictionary<String, EndpointSettings>
                        {
                            ["rfid"] = new EndpointSettings
                            {
                                IPAddress = "172.18.0.3"
                            }
                        }
                    }
                });

                await client.Containers.StartContainerAsync(containerName, new ContainerStartParameters());

                // the sql server needs some time before the first connection to be open
                while (true)
                {
                    try
                    {
                        using (var connection = RfidDatabase.CreateConnection())
                        {
                            await connection.OpenAsync();
                        }

                        break;
                    }
                    catch
                    {
                        await Task.Delay(4000);
                    }
                }
            }
        }
    }
}
