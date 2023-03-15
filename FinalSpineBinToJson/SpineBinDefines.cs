
namespace FinalHogen.spine.bincode.v36x
{
  public enum BoneNum{
    BONE_ROTATE = 0,
    BONE_SCALE   = 1 ,
    BONE_SHEAR   = 2 ,
  }
  public enum SlotNum{
    SLOT_ATTACHMENT = 0,
    SLOT_COLOR           = 1,
    SLOT_TWO_COLOR   = 2,
  }
  public enum PathNum{
    PATH_POSITION = 0,
    PATH_SPACING   = 1,
    PATH_MIX          = 2,
  }
  public enum CurveNum{
    CURVE_LINEAR    = 0,
    CURVE_STEPPED  = 1,
    CURVE_BEZIER    = 2,
  }
	public enum TransformMode {
		//0000 0 Flip Scale Rotation
		Normal = 0, // 0000
		OnlyTranslation = 7, // 0111
		NoRotationOrReflection = 1, // 0001
		NoScale = 2, // 0010
		NoScaleOrReflection = 6, // 0110
	}
	public enum BlendMode {
		Normal, Additive, Multiply, Screen
	}
	public enum PositionMode {
		Fixed, Percent        
	}

	public enum SpacingMode {
		Length, Fixed, Percent
	}

	public enum RotateMode {
		Tangent, Chain, ChainScale
	}
	public enum BoneTimelineType {
		Rotate= 0,
		Translate = 1,
		Scale = 2,
		Shear = 3,
	}
	public enum SlotTimelineType {
		Attachment = 0,
		Color = 1,
		TwoColor = 2,
	}
	public enum PathTimelineType {
		Position = 0,
		Spacing = 1,
		Mix = 2,
	}
	public enum CurveTimelineType {
		Linear = 0,
		Stepped = 1,
		Bezier = 2,
	}
	
	public enum AttachmentType {
		Region, Boundingbox, Mesh, Linkedmesh, Path, Point, Clipping
	}
  public class StaticValues{
		public static readonly TransformMode[] TransformModeValues = {
			TransformMode.Normal,
			TransformMode.OnlyTranslation,
			TransformMode.NoRotationOrReflection,
			TransformMode.NoScale,
			TransformMode.NoScaleOrReflection
		};
  }

}