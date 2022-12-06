using System.Collections;
using System.Collections.Generic;
using System;
using Sunbox.Avatars;
using UnityEngine;
using System.IO;


//무언가 에러나면 다른 스크립트 파일에 using Sunbox.Avatars; 추가하기
public class AvatarManager : MonoBehaviour
{
    public AvatarReferences AvatarReferences;

    public AvatarCustomization GetAvatarCustomization(GameObject instance)
    {
        AvatarCustomization ac = instance.GetComponent<AvatarCustomization>();
        return ac;
    }

    public string LoadCharData(string filename)
    {
        string path = CharNameToPath(filename);

        if(!File.Exists(path))
        {
            return "";
        }

        string data = File.ReadAllText(path);
        return data;
    }
    string CharNameToPath(string charName) => Path.Join(GlobalVariables.CharacterPath, charName + ".sv");

    public string GetConfigString(AvatarCustomization ac)
    {
        string json = AvatarCustomization.ToConfigString(ac);
        return json;
    }
    public bool ApplyAvatarCustomization(string json, AvatarCustomization avatar)
    {
        avatar.ClothingItemHat = null;
        avatar.ClothingItemTop = null;
        avatar.ClothingItemBottom = null;
        avatar.ClothingItemGlasses = null;
        avatar.ClothingItemShoes = null;
        foreach (string str in json.Split('\n'))
        {
            var type = str.Split('=')[0];
            if (type == "") break;
            var value = str.Split('=')[1];
            float floatValue; int intValue;
            float.TryParse(value, out floatValue);
            int.TryParse(value, out intValue);
            switch (type)
            {
                case "g_gender":
                    if (value == "Male")
                        avatar.SetGender(AvatarCustomization.AvatarGender.Male);
                    else if (value == "Female")
                        avatar.SetGender(AvatarCustomization.AvatarGender.Female);
                    break;
                case "f_bodyFat":
                    avatar.BodyFat = floatValue;
                    break;
                case "f_bodyMuscle":
                    avatar.BodyMuscle = floatValue;
                    break;
                case "f_bodyHeightMetres":
                    avatar.BodyHeight = floatValue;
                    break;
                case "f_breastSize":
                    avatar.BreastSize = floatValue;
                    break;
                case "f_noseLength":
                    avatar.NoseLength = floatValue;
                    break;
                case "f_lipsWidth":
                    avatar.LipsWidth = floatValue;
                    break;
                case "f_jawWidth":
                    avatar.JawWidth = floatValue;
                    break;
                case "f_browWidth":
                    avatar.BrowWidth = floatValue;
                    break;
                case "f_browHeight":
                    avatar.BrowHeight = floatValue;
                    break;
                case "f_eyesSize":
                    avatar.EyesSize = floatValue;
                    break;
                case "f_eyesClosedDefault":
                    avatar.EyesClosedDefault = floatValue;
                    break;
                case "i_skinMaterialIndex":
                    avatar.SkinMaterialIndex = intValue;
                    break;
                case "i_hairStyleIndex":
                    avatar.HairStyleIndex = intValue;
                    break;
                case "i_hairMaterialIndex":
                    avatar.HairMaterialIndex = intValue;
                    break;
                case "i_facialHairStyleIndex":
                    avatar.FacialHairStyleIndex = intValue;
                    break;
                case "i_facialHairMaterialIndex":
                    avatar.FacialHairMaterialIndex = intValue;
                    break;
                case "i_eyeMaterialIndex":
                    avatar.EyeMaterialIndex = intValue;
                    break;
                case "i_lashesMaterialIndex":
                    avatar.LashesMaterialIndex = intValue;
                    break;
                case "i_browMaterialIndex":
                    avatar.BrowMaterialIndex = intValue;
                    break;
                case "i_nailsMaterialIndex":
                    avatar.NailsMaterialIndex = intValue;
                    break;
                case "clothingItem":
                    string itemStr = value.Split('-')[0];
                    ClothingItem item = Array.Find(AvatarReferences.AvailableClothingItems, item => item.Name == itemStr);
                    int varIndex = int.Parse(value.Split('-')[1]);
                    avatar.AttachClothingItem(
                        item: item,
                        variationIndex: varIndex
                    );
                    break;
                default:
                    break;
            }
        }
        avatar.UpdateCustomization();
        avatar.UpdateClothing();
        return true;
    }
}
