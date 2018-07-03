using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> _music;
    public List<AudioClip> _menu;
    public List<AudioClip> _battle;

    public AudioSource _a1_music;
    public AudioSource _a2_menus;
    public AudioSource _a3_se1;
    public AudioSource _a4_se2;
    public AudioSource _a5_se3;
    public AudioSource _a6_battle;
    public AudioSource _a7_game;

    public bool _mute = false;
    private int _menuIndex = -1;

    [Header("Specifics")]
    public AudioClip _confirmSound;
    public AudioClip _cancelSound;
    public AudioClip _playerTurnSound;
    public AudioClip _enemyTurnSound;
    public AudioClip _gameStartSound;
    public AudioClip _selectionChangeSound;
    public AudioClip _dialogueAdvanceSound;
    public AudioClip _playerDefeatSound;
    public AudioClip _enemyDefeatSound;
    public AudioClip _missionOverSound;
    public AudioClip _missionWinSound;

    public void PlayMusic(bool play, bool newTrack, int index)
    {
        if (_a1_music != null)
        {
            if (play && !_mute)
            {
                if (newTrack)
                {
                    if (_music.Count > 0 && _music.Count > index)
                    {
                        _a1_music.clip = _music[index];
                        _a1_music.Play();
                    }
                }
                else
                {
                    _a1_music.UnPause();
                }
                _a2_menus.Pause();
                _a6_battle.Pause();
            }
            else
            {
                _a1_music.Pause();
            }
        }
    }

    public void PlayBattleMusic(bool play, bool newTrack)
    {
        if (_a6_battle != null)
        {
            if (play && !_mute)
            {
                if (newTrack)
                {
                    _a6_battle.clip = _battle[0];
                    _a6_battle.Play();
                }
                else
                {
                    _a6_battle.UnPause();
                }
                _a2_menus.Pause();
                _a1_music.Pause();
            }
            else
            {
                _a6_battle.Pause();
            }
        }
    }

    public void PlayMenu(bool play)
    {
        if (_a2_menus != null)
        {
            if (play && !_mute)
            {
                if (_menuIndex == -1)
                {
                    _a2_menus.clip = _menu[0];
                    _a2_menus.Play();
                    _menuIndex = 0;
                }
                else
                {
                    _a2_menus.UnPause();
                }
                _a1_music.Pause();
                _a6_battle.Pause();
            }
            else
            {
                _a2_menus.Pause();
            }
        }
    }

    public void PlaySE(bool isplayer, AudioClip CharacterSE)
    {
        if (!_mute)
        {
            if (isplayer)
            {
                if (_a3_se1 != null)
                {
                    _a3_se1.clip = CharacterSE;
                    _a3_se1.Play();
                }
            }
            else
            {
                if (_a4_se2 != null)
                {
                    _a4_se2.clip = CharacterSE;
                    _a4_se2.Play();
                }
            }
        }
    }

    public void PlayGameSE(AudioClip SE)
    {
        if (_a5_se3 != null)
        {
            _a5_se3.clip = SE;
            _a5_se3.Play();
        }
    }

    public void PlaySpecialGameSE(AudioClip SE)
    {
        if (_a7_game != null && !_mute)
        {
            _a7_game.clip = SE;
            _a7_game.Play();
        }
    }

    public void PlayConfirmation()
    {
        PlayGameSE(_confirmSound);
    }

    public void PlayCancelSound()
    {
        PlayGameSE(_cancelSound);
    }

    public void PlayerTurnSound()
    {
        PlaySpecialGameSE(_playerTurnSound);
    }

    public void PlayEnemyTurnSound()
    {
        PlaySpecialGameSE(_enemyTurnSound);
    }

    public void PlayGameStartSound()
    {
        PlaySpecialGameSE(_gameStartSound);
    }

    public void PlayerDefeatSound()
    {
        PlaySpecialGameSE(_playerDefeatSound);
    }

    public void EnemyDefeatSound()
    {
        PlaySpecialGameSE(_enemyDefeatSound);
    }

    public void PlaySelectionChangeSound()
    {
        PlayGameSE(_selectionChangeSound);
    }

    public void PlayDialogueAdvanceSound()
    {
        PlayGameSE(_dialogueAdvanceSound);
    }

    public void PlayMissionWinSound()
    {
        PlaySpecialGameSE(_missionWinSound);
    }

    public void PlayMissionOverSound()
    {
        PlaySpecialGameSE(_missionOverSound);
    }

    public void MuteAll()
    {
        _mute = true;
        //if (_a1_music != null)
        //    _a1_music.Pause();
        //if (_a6_battle != null)
        //    _a6_battle.Pause();
        if (_a2_menus != null)
            _a2_menus.Pause();
    }

    public void UnMute()
    {
        _mute = false;
        if (_a2_menus != null)
            _a2_menus.UnPause();
        // not finished
    }
}
