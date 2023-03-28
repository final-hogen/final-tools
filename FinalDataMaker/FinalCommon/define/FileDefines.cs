
namespace FinalHogen.gear{
  public static class GearFiles{
    static string OutputJsonDirectry = "出力json";
    static Dictionary<string,string> finalFiileNames = new Dictionary<string,string>(){
      {"GirlData","datass2.txt.JsonData/118-GirlData.json"},
      {"GirlSkin","datass2.txt.JsonData/119-GirlSkinData.json"},
      {"GirlAwakeData","datass2.txt.JsonData/117-GirlAwakeData.json"},
      {"GirlStarData","datass2.txt.JsonData/120-GirlStarData.json"},
      {"GirlVoice","datass2.txt.JsonData/296-VoiceData.json"},

      {"SkillData","datass2.txt.JsonData/287-TrunkSkillData.json"},
      {"SkillDesc","datass2.txt.JsonData/287-TrunkSkillData.json"},
      {"SkillGroup","datass2.txt.JsonData/222-SkillArrayData.json"},
      
      {"LevelUpData","datass2.txt.JsonData/122-GirllevelData.json"},
      {"TalentRewardData","datass2.txt.JsonData/274-TalentRewardData.json"},
      {"McvNatureID","datass2.txt.JsonData/143-MCVNatureIdData.json"},
      {"McvGirlSkill","datass2.txt.JsonData/144-MCVNatureUnlockSkillData.json"},
      {"McvSkillData","datass2.txt.JsonData/140-MCVGirlSkillData.json"},

      {"LangData","JAP_LanguageData.txt.JsonData/000-.json"},
      {"CNLangData","SC_LanguageData.txt.JsonData/000-.json"},
      {"KeyToText","datass2.txt.JsonData/132-KeyToTextData.json"},
      {"PropertyData","datass2.txt.JsonData/188-PropertyData.json"},
      {"SystemConfigData","datass2.txt.JsonData/273-SystemConfigData.json"},
      {"BattleConfigData","datass.txt.JsonData/003-BattleConfigData.json"},

      {"ItemData","datass2.txt.JsonData/131-ItemData.json"},
      
      {"WidgetData","datass2.txt.JsonData/308-WidgetData.json"},
      {"WidgetIntroData","datass2.txt.JsonData/310-WidgetIntroData.json"},
      {"SuitData","datass2.txt.JsonData/264-SuitData.json"},
      {"MachineArmorData","datass2.txt.JsonData/159-MachineArmorData.json"},

      {"EquipBagData","datass.txt.JsonData/007-EquipBagData.json"},
      {"EquipBodyData","datass.txt.JsonData/009-EquipBodyData.json"},
      {"EquipLegData","datass.txt.JsonData/018-EquipLegData.json"},
      {"EquipArmData","datass.txt.JsonData/021-EquipArmData.json"},
      {"BulletData","datass.txt.JsonData/020-BulletData.json"},
      
      {"StarShipWepon","datass2.txt.JsonData/260-StarshipsubsidiarycannonData.json"},
      {"StarShipBuff","datass2.txt.JsonData/259-StarshipbuffData.json"},
      };
    public static void CheckFiles(){
      CheckFiles(GearFiles.finalFiileNames);
    }
    public static void CheckFiles(Dictionary<string,string> fiileNames){
      foreach(KeyValuePair<string,string> keyval in finalFiileNames){
        if(!File.Exists(keyval.Value)){
          throw new FileNotFoundException(keyval.Value);
        }
      }
    }
  }
}
