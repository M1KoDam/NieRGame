using UnityEngine;

namespace Menu.AudioManager.Scripts
{
    public class FlightSideMusic: MonoBehaviour
    {
        public Sounds sounds;

        private bool _isSoundPlaying;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (sounds.AllSounds is not null && !_isSoundPlaying)
            {
                _isSoundPlaying = true;   
                sounds.AllSounds["FlightSideMusic"].PlaySoundLoop();
            }
        }
    }
}