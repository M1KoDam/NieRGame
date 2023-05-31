using UnityEngine;

namespace Menu.AudioManager.Scripts
{
    public class FlightSideMusic: MonoBehaviour
    {
        public Musics musics;

        private bool _isSoundPlaying;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (musics.AllMusics is not null && !_isSoundPlaying)
            {
                _isSoundPlaying = true;   
                musics.AllMusics["FlightSideMusic"].PlaySoundLoop();
            }
        }
    }
}