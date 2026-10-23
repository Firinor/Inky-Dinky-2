using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
   public static SoundManager Instance;
   [SerializeField]
   private AudioConfig config;
   [SerializeField]
   private List<AudioSource> audioPool;
   private int soundIndex;

   private const float buttonClickDelay = .2f;
   private DateTime lastButtonClick;

   private AudioSource scoresSource;
   
   private void Awake()
   {
      Instance = this;
      lastButtonClick = DateTime.Now;
   }
   
   public void PlayButtonClick(Vector3 position = default)
   {
      if((DateTime.Now - lastButtonClick).TotalSeconds < buttonClickDelay)
         return;
      
      lastButtonClick = DateTime.Now;
      
      Play(position, config.ButtonClick, isPriority: true);
   }
   public void PlayOpenScroll(Vector3 position = default)
   {
      Play(position, config.OpenScroll, isPriority: true);
   }
   public void PlayCloseScroll(Vector3 position = default)
   {
      Play(position, config.CloseScroll, isPriority: true);
   }
   public void PlayHelpSpell(Vector3 position = default)
   {
      Play(position, config.HelpSpell, isPriority: true);
   }
   public void PlayTileStartCollide(Vector3 position = default)
   {
      Play(position, config.StartCollide, isPriority: true);
   }
   public void PlayTileEndCollide(Vector3 position = default)
   {
      Play(position, config.EndCollide, isPriority: true);
   }
   public void PlayTileSelect(Vector3 position = default, float volumeMultiplier = 1f)
   {
      Play(position, config.TileSelect, volumeMultiplier: volumeMultiplier);
   }
   public void PlayTileError(Vector3 position = default)
   {
      Play(position, config.TileError);
   }
   
   public void PlayWin(Vector3 position = default)
   {
      Play(position, config.Win, isPriority: true);
   }
   public void PlayLose(Vector3 position = default)
   {
      Play(position, config.Lose, isPriority: true);
   }
   public void PlayLevel(Vector3 position = default)
   {
      Play(position, config.Level, isPriority: true);
   }
   public void PlayUnlock(Vector3 position = default)
   {
      Play(position, config.Unlock, isPriority: true);
   }
   public void PlayScores()
   {
      scoresSource = audioPool[soundIndex];

      soundIndex++;
      soundIndex %= audioPool.Count;
   
      ClipSettings clipData = config.Scores;
      
      scoresSource.gameObject.SetActive(true);
      scoresSource.transform.position = default;
      scoresSource.pitch = 1 + Random.Range(-0.05f, 0.05f);
      scoresSource.clip = clipData.Clip;
      scoresSource.volume = clipData.Volume;
      scoresSource.loop = true;
      scoresSource.Play();
   }
   public void StopScores()
   {
      scoresSource.Stop();
      scoresSource.gameObject.SetActive(false);
      scoresSource = null;
   }
   
   public void Play(Vector3 position, ClipSettings clipData, bool isPriority = false, float volumeMultiplier = 1)
   {
      AudioSource source = audioPool.FirstOrDefault(a => !a.gameObject.activeSelf);

      if (source is null)
      {
         source = audioPool[soundIndex];
      }

      soundIndex++;
      soundIndex %= audioPool.Count;

      source.gameObject.SetActive(true);
      source.transform.position = position;
      source.pitch = 1 + Random.Range(-0.05f, 0.05f);
      source.volume = clipData.Volume * volumeMultiplier;
      
      source.PlayOneShot(clipData.Clip);

      StartCoroutine(DisableAudioSource(source));
   }

   private IEnumerator DisableAudioSource(AudioSource source)
   {
      while (source.isPlaying)
      {
         yield return null;
      }
      
      source.gameObject.SetActive(false);
   }
}
