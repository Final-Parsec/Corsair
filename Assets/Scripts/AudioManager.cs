using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioSource trumpAudio;
    private AudioSource guiAudio;
    private AudioSource otherAudio;

    // For GUI
    public AudioClip button;
    public AudioClip sell;

    // For Trump
    public AudioClip win;
    public AudioClip loss;
    public AudioClip noMoney;
    public AudioClip cantBuildThere;
    public AudioClip hillary;
    public AudioClip bernie;
    public AudioClip obama;
    public AudioClip[] waveMexican;
    public AudioClip[] waveChinese;
    public AudioClip[] waveMuslim;

    private int mexicanIndex;
    private int chineseIndex;
    private int muslimIndex;
    private ObjectManager objectManager;
    
    public delegate void TalkingAction();
    public event TalkingAction Talking;

    // Use this for initialization
    void Start()
    {
        objectManager = ObjectManager.GetInstance();
        var audios = GetComponents<AudioSource>();
        trumpAudio = audios[0];
        guiAudio = audios[1];
        otherAudio = audios[2];
        mexicanIndex = Random.Range(0, waveMexican.Length);
        chineseIndex = Random.Range(0, waveChinese.Length);
        muslimIndex = Random.Range(0, waveMuslim.Length);
    }

    public void PlayWaveSound(WaveId waveId)
    {
        switch (waveId)
        {
            case WaveId.FastMexicanMen:
            case WaveId.AnchorMexicanWomen:
                PlayTrump(waveMexican[mexicanIndex++ % waveMexican.Length], false);
                break;

            case WaveId.GroupChineseMen:
            case WaveId.AnchorChineseWomen:
                PlayTrump(waveChinese[chineseIndex++ % waveChinese.Length], false);
                break;

            case WaveId.StrongMuslimMen:
            case WaveId.AnchorMuslimWomen:
                PlayTrump(waveMuslim[muslimIndex++ % waveMuslim.Length], false);
                break;

            case WaveId.Hillary:
                PlayOther(hillary, false);
                break;

            case WaveId.Bernie:
                PlayOther(bernie, false);
                break;

            case WaveId.Obama:
                PlayOther(obama, false);
                break;
        }
    }

    public void PlayEndGame(bool winCondition)
    {
        if (winCondition)
        {
            PlayTrump(win, true);
        }
        else
        {
            PlayTrump(loss, true);
        }
    }

    public void PlayMoney()
    {
        PlayTrump(noMoney, false);
    }

    public void PlayBlock()
    {
        PlayTrump(cantBuildThere, false);
    }

    

    public void PlayButtonSound()
    {
        PlayGui(button, false);
    }

    public void PlaySellSound()
    {
        PlayGui(sell, false);
    }

    private void PlayTrump(AudioClip newClip, bool isPrimeAudio)
    {
        if ((!trumpAudio.isPlaying || isPrimeAudio) && !objectManager.gameState.isMuted)
        {
            // Call callback
            if (this.Talking != null)
            {
                this.Talking();
            }

            trumpAudio.Stop();
            trumpAudio.clip = newClip;
            trumpAudio.Play();
        }
    }

    private void PlayGui(AudioClip newClip, bool isPrimeAudio)
    {
        if ((!guiAudio.isPlaying || isPrimeAudio) && !objectManager.gameState.isMuted)
        {
            guiAudio.Stop();
            guiAudio.clip = newClip;
            guiAudio.Play();
        }
    }

    private void PlayOther(AudioClip newClip, bool isPrimeAudio)
    {
        if ((!otherAudio.isPlaying || isPrimeAudio) && !objectManager.gameState.isMuted)
        {
            otherAudio.Stop();
            otherAudio.clip = newClip;
            otherAudio.Play();
        }
    }
}
