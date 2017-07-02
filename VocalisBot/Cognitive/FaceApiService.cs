using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace VocalisBot.Cognitive
{
    public static class FaceApiService
    {
        private static FaceServiceClient _faceServiceClient = new FaceServiceClient(Constants.FACE_API_KEY, Constants.FACE_API_ROOT);

        public static async Task<FaceAttributes> DetectFaceAttributesAsync(string url)
        {
            try
            {
                var requiredFaceAttributes = new FaceAttributeType[] {
                    FaceAttributeType.Age, FaceAttributeType.Gender
                };

                var faces = await _faceServiceClient.DetectAsync(url, returnFaceAttributes: requiredFaceAttributes);
                return faces[0].FaceAttributes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Face[]> DetectFacesAndGenderAsync(string url)
        {
            try
            {
                var requiredFaceAttributes = new FaceAttributeType[] {
                    FaceAttributeType.Gender
                };

                return await _faceServiceClient.DetectAsync(url, returnFaceAttributes: requiredFaceAttributes);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}