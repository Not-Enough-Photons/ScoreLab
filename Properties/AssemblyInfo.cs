using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(NEP.Scoreworks.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(NEP.Scoreworks.BuildInfo.Company)]
[assembly: AssemblyProduct(NEP.Scoreworks.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NEP.Scoreworks.BuildInfo.Author)]
[assembly: AssemblyTrademark(NEP.Scoreworks.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(NEP.Scoreworks.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NEP.Scoreworks.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(NEP.Scoreworks.Main), NEP.Scoreworks.BuildInfo.Name, NEP.Scoreworks.BuildInfo.Version, NEP.Scoreworks.BuildInfo.Author, NEP.Scoreworks.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]