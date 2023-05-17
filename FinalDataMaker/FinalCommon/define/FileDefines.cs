
using System.Text.Json.Nodes;
using FinalHogen.json;

namespace FinalHogen.gear{
  internal class GearFileMaker{
    private Dictionary<string,JsonArray> keysData = new Dictionary<string, JsonArray>();
    public readonly Dictionary<string,string> filepathData = new Dictionary<string, string>();
    protected JsonArray GetKeyFile(string keyFilepath){
      if(keysData.ContainsKey(keyFilepath))return keysData[keyFilepath];
      JsonNode? result = JsonFile.load(keyFilepath);
      if(result==null)throw new FileNotFoundException(keyFilepath);
      JsonArray array = result.AsArray();
      keysData[keyFilepath] = array;
      return array;
    }
    protected string FindFile(JsonArray array, string filename){
      foreach(JsonNode? node in array){
        if(node==null)continue;
        string check = (string)node!;
        string name = check.Substring(check.IndexOf('-')+1);
        if(filename==name)return check;
      }
      throw new FileNotFoundException(filename);
    }
    public void LoadFilePaths(){
      foreach(KeyValuePair<string,string> keyval in finalFiileNames){
        LoadFilePath(keyval.Key,keyval.Value);
      }
    }
    protected void LoadFilePath(string name, string file){
      int lastslash = file.LastIndexOf('/');
      string key = file.Substring(0,lastslash);
      string folder = key+".JsonData/";
      string keyFilepath = folder+key+".keys.json";
      string filename = file.Substring(lastslash+1);
      JsonArray list = GetKeyFile(keyFilepath);
      string filePath = folder+FindFile(list,filename)+".json";
      filepathData[name] = filePath;
    }
    protected static readonly Dictionary<string,string> finalFiileNames = new Dictionary<string,string>(){
      {"GirlData","datass2.txt/GirlData"},
      {"GirlSkin","datass2.txt/GirlSkinData"},
      {"GirlAwakeData","datass2.txt/GirlAwakeData"},
      {"GirlStarData","datass2.txt/GirlStarData"},
      {"GirlVoice","datass2.txt/VoiceData"},

      {"SkillData","datass2.txt/TrunkSkillData"},
      {"SkillDesc","datass2.txt/TrunkSkillData"},
      {"SkillGroup","datass2.txt/SkillArrayData"},
      
      {"LevelUpData","datass2.txt/GirllevelData"},
      {"TalentRewardData","datass2.txt/TalentRewardData"},
      {"McvNatureID","datass2.txt/MCVNatureIdData"},
      {"McvGirlSkill","datass2.txt/MCVNatureUnlockSkillData"},
      {"McvSkillData","datass2.txt/MCVGirlSkillData"},

      {"LangData","JAP_LanguageData.txt/"},
      {"CNLangData","SC_LanguageData.txt/"},

      {"StoryMessage","JAP_storyJsonDatas.txt/"},
      {"StoryAction","datass.txt/DialogActionDatas"},

      {"KeyToText","datass2.txt/KeyToTextData"},
      {"PropertyData","datass2.txt/PropertyData"},
      {"SystemConfigData","datass2.txt/SystemConfigData"},
      {"BattleConfigData","datass.txt/BattleConfigData"},

      {"ItemData","datass2.txt/ItemData"},
      
      {"WidgetData","datass2.txt/WidgetData"},
      {"WidgetIntroData","datass2.txt/WidgetIntroData"},
      {"SuitData","datass2.txt/SuitData"},
      {"MachineArmorData","datass2.txt/MachineArmorData"},

      {"EquipBagData","datass.txt/EquipBagData"},
      {"EquipBodyData","datass.txt/EquipBodyData"},
      {"EquipLegData","datass.txt/EquipLegData"},
      {"EquipArmData","datass.txt/EquipArmData"},
      {"BulletData","datass.txt/BulletData"},
      
      {"StarShipWepon","datass2.txt/StarshipsubsidiarycannonData"},
      {"StarShipBuff","datass2.txt/StarshipbuffData"},
      };

  }
  public static class GearFiles{
    public static readonly string OutputJsonDirectry = "出力json/";
    public static Dictionary<string,string> MakeFinalfilenames(){
      GearFileMaker maker = new GearFileMaker();
      maker.LoadFilePaths();
      return maker.filepathData;
    }
  }
}
