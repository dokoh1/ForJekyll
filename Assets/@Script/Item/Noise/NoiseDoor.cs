using UnityEngine;

namespace Item
{
    public class NoiseDoor : NoiseObject
    {
        public bool isOpen;

        public override void Interact()
        {
            if (!isOpen)
            {
                isOpen = true;
                // �� ������ ����
                Debug.Log($"�� ����");
                PlayNoise(TapNoiseTimer, 0);
            }
            else
            {
                isOpen = false;
                // �� ������ ����
                Debug.Log($"�� ����");
                PlayNoise(TapNoiseTimer, 1);
            }
        }
    }
}

