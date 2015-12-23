using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource trumpAudio;
    private AudioSource guiAudio;

    // For GUI
    public AudioClip button;
    public AudioClip sell;

    // For Trump
    public AudioClip win;
    public AudioClip loss;
    public AudioClip noMoney;
    public AudioClip cantBuildThere;
    public AudioClip[] waveMexican;
    public AudioClip[] waveChinese;
    public AudioClip[] waveMuslim;

    private int mexicanIndex;
    private int chineseIndex;
    private int muslimIndex;
    private ObjectManager objectManager;

    // Use this for initialization
    void Start()
    {
        objectManager = ObjectManager.GetInstance();
        var audios = GetComponents<AudioSource>();
        trumpAudio = audios[0];
        guiAudio = audios[1];
        mexicanIndex = Random.Range(0, waveMexican.Length);
        chineseIndex = Random.Range(0, waveChinese.Length);
        muslimIndex = Random.Range(0, waveMuslim.Length);
    }

    public void PlayWaveSound(WaveId waveId)
    {
        switch (waveId)
        {
            case WaveId.Zombie:
                PlayTrump(waveMexican[mexicanIndex++ % waveMexican.Length], false);
                break;

            case WaveId.Horde:
                PlayTrump(waveChinese[chineseIndex++ % waveChinese.Length], false);
                break;

            case WaveId.PregerWench:
                PlayTrump(waveMuslim[muslimIndex++ % waveMuslim.Length], false);
                break;

            case WaveId.SeaDog:
                PlayTrump(waveChinese[chineseIndex++ % waveChinese.Length], false);
                break;

            case WaveId.Dog:
                PlayTrump(waveMuslim[muslimIndex++ % waveMuslim.Length], false);
                break;

            case WaveId.Seagull:
                PlayTrump(waveChinese[chineseIndex++ % waveChinese.Length], false);
                break;

            case WaveId.Boss3:
            case WaveId.Boss2:
            case WaveId.Boss1:
                PlayTrump(waveMexican[mexicanIndex++ % waveMexican.Length], false);
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
}
