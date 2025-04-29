namespace ProjektGrupowy.API.Utils;

public static class DockerDetector
{
    public static bool IsRunningInDocker()
    {
        return File.Exists("/.dockerenv") || 
               File.Exists("/proc/self/cgroup") && 
               File.ReadAllText("/proc/self/cgroup").Contains("docker");
    }
}