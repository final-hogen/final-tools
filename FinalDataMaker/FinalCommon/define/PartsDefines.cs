
namespace FinalHogen.gear
{
  /// <summary>
  ////パーツタイプ。ゲームと同じ並び。変えてはならない。
  /// </summary>
  public enum EquipType
	{
		武器,
		胸部,
		脚部,
		背部,
		チップ,
		なし
	}
  /// <summary>
  ////パーツサブタイプ。ゲームと同じ並び。変えてはならない。
  /// </summary>
	public enum EquipSubType
	{
		防壁武器,
		片手武器,
		突撃武器,
		強襲武器,
		機銃武器,
		突進武器,
		狙撃武器,
		炸裂武器,
		投射武器,
		軽型攻撃胸部,
		中型攻撃胸部,
		重型攻撃胸部,
		中型生存胸部,
		重型生存胸部,
		軽型攻撃脚部,
		中型攻撃脚部,
		重型攻撃脚部,
		中型生存脚部,
		重型生存脚部,
		軽型攻撃背部,
		中型攻撃背部,
		重型攻撃背部,
		中型生存背部,
		重型生存背部,
		生存チップ,
		攻撃チップ,
		なし,
		特殊チップ
	}
  /// <summary>
  ////弾丸タイプ。ゲームと同じ並び。変えてはならない。
  /// </summary>
  public enum BulletType
  {
    弾丸,
    レーザー,
    炸裂弾,
    投射弾,
    近接攻撃,
    持続
  }

/// <summary>
/// 入手時オプションの設定タイプ。ゲームと同じ並び。変えてはならない。
/// </summary>
	public enum PropertyType
	{
		なし,
		ランダムオプション, // 中国ガチャ
		固定オプション,   //←日本版のガチャ交換
		ランダム4つオプション //新しいガチャ？
	}
	public static class FinalPartsDefineExtention{
		public static string ToName(this EquipSubType type){
			switch(type){
				case EquipSubType.軽型攻撃胸部:
				return "軽型胸部";
				case EquipSubType.軽型攻撃脚部:
				return "軽型脚部";
				case EquipSubType.軽型攻撃背部:
				return "軽型背部";
				case EquipSubType.中型攻撃胸部:
				case EquipSubType.中型生存胸部:
				return "中型胸部";
				case EquipSubType.中型攻撃脚部:
				case EquipSubType.中型生存脚部:
				return "中型脚部";
				case EquipSubType.中型攻撃背部:
				case EquipSubType.中型生存背部:
				return "中型背部";
				case EquipSubType.重型攻撃胸部:
				case EquipSubType.重型生存胸部:
				return "重型胸部";
				case EquipSubType.重型攻撃脚部:
				case EquipSubType.重型生存脚部:
				return "重型脚部";
				case EquipSubType.重型攻撃背部:
				case EquipSubType.重型生存背部:
				return "重型背部";
			}
			return type.ToString();
		}
	}
	/// <summary>
	/// 生産陣営タイプ。ゲームと同じ並び。変えてはならない。
	/// </summary>
	public enum WidgetCamp
	{
		なし,
		一般,
		ブラックアーク,
		パラノイド,
		ヘルハピ,
		ハイクシアー,
		アイレタ,
		ケゲア,
		無所属,
		Nerv,
		WILLE,
		WIlle
	}
}
