using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace VocalisBot.Cognitive
{
    public static class ComputerVisionService
    {
        private static VisionServiceClient _visionServiceClient = new VisionServiceClient(Constants.COMPUTER_VISION_KEY, Constants.COMPUTER_VISION_ROOT);

        public static async Task<Caption> DescribeAsync(string url)
        {
            var analysisResult = await _visionServiceClient.DescribeAsync(url);
            return analysisResult.Description.Captions[0];
        }
    }
}