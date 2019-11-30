using System.Collections.Generic;
using System.Linq;

namespace MetaSprite
{
    public class AnimationClip
    {
        public readonly AsepriteWrapMode wrapMode;
        public readonly bool loopTime;
        public readonly string Name;

        public IReadOnlyList<Sprite> Frames => spriteFrameList;
        List<Sprite> spriteFrameList = new List<Sprite>();
        public IReadOnlyList<float> ElapsedTimeList => elapsedTimeList;
        public IReadOnlyList<float> ElapsedStartTimeList => elapsedStartTimeList;
        public float Duration => duration;
        List<float> elapsedTimeList = new List<float>();
        List<float> elapsedStartTimeList = new List<float>();
        float duration;

        #region frame info
        public AnimationClip(FrameTag tag, List<Sprite> sprites, List<Frame> frames)
        {
            Name = tag.name;

            // Set loop property
            loopTime = tag.properties.Contains("loop");

            var normalFrameList = new List<Sprite>((tag.to - tag.from + 1) * 2);
            for (int i = tag.from; i <= tag.to; ++i) normalFrameList.Add(sprites[i]);

            // set frame
            if (tag.properties.Contains("reverse"))  // ex: 4, 3, 2, 1
            {
                wrapMode = AsepriteWrapMode.Reverse;
                normalFrameList.Reverse();
                spriteFrameList = normalFrameList;

            }
            else if (tag.properties.Contains("ping-pong")) // ex: 1, 2, 3, 4, 3, 2
            {
                wrapMode = AsepriteWrapMode.PingPongLoop;
                spriteFrameList = new List<Sprite>(normalFrameList);
                spriteFrameList.AddRange(normalFrameList.Skip(1).Take(normalFrameList.Count - 2).Reverse());
            }
            else // ex: 1, 2, 3, 4
            {
                wrapMode = AsepriteWrapMode.Normal;
                spriteFrameList = normalFrameList; // do noting ...
            }


            elapsedTimeList = new List<float>(spriteFrameList.Count);
            elapsedStartTimeList = new List<float>(spriteFrameList.Count);
            float elapsedTime = 0;
            foreach (var item in spriteFrameList)
            {
                elapsedStartTimeList.Add(elapsedTime);
                elapsedTime += item.duration;
                elapsedTimeList.Add(elapsedTime);
            }

            duration = elapsedTime;
        }
        #endregion


        public int FindFrame(float tt)
        {
            // ���ַ����в���
            var list = ElapsedTimeList;
            var g = tt % list[list.Count - 1];
            int max = list.Count - 1;

            // ���ַ����ң��� timeElased С�� index
            int l = 0;
            int r = max;
            int m = (l + r) / 2;

            while (l < r)
            {
                var v = list[m];
                if (g > v)
                {
                    l = m;
                }
                else
                {
                    r = m;
                }

                m = (l + r) / 2;
            }

            return m;
        }

        #region subimage
        public bool isSubImage => SubImageTagName != null;
        public string SubImageTagName;
        readonly Dictionary<string, AnimationClip> SubImage = new Dictionary<string, AnimationClip>();
        #endregion



        #region draw region
        #endregion
    }
}