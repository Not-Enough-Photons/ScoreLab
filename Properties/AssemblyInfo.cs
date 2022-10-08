using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(NEP.ScoreLab.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(NEP.ScoreLab.BuildInfo.Company)]
[assembly: AssemblyProduct(NEP.ScoreLab.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NEP.ScoreLab.BuildInfo.Author)]
[assembly: AssemblyTrademark(NEP.ScoreLab.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(NEP.ScoreLab.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NEP.ScoreLab.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(NEP.ScoreLab.Main), NEP.ScoreLab.BuildInfo.Name, NEP.ScoreLab.BuildInfo.Version, NEP.ScoreLab.BuildInfo.Author, NEP.ScoreLab.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]