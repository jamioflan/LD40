using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public Image arrow;
    public Image ping;
    public GameObject gameOverBox;
    public Text gameOverText;

    public Vector3 pingTarget = Vector3.zero;
    public float fPingProgress = 10.0f;

	// Use this for initialization
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        // Draw an arrow to where the base is
        WorldTile baseTile = Core.OuEstLeBase(true);
        if (baseTile != null)
        {
            Vector3 deltaPos = baseTile.transform.position + new Vector3(5.0f, 0.0f, 5.0f) - Core.GetCore().thePlayer.transform.position;

            arrow.enabled = deltaPos.magnitude >= 10.0f;
            float fAngle = Mathf.Atan2(deltaPos.z, deltaPos.x) * Mathf.Rad2Deg;
            arrow.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, fAngle - 90.0f);
        }

        if (fPingProgress < 2.0f)
        {
            fPingProgress += Time.deltaTime;

            Vector3 deltaPos = pingTarget - Core.GetCore().thePlayer.transform.position;

            ping.enabled = true;
            ping.rectTransform.localScale = new Vector3(fPingProgress + 1.0f, fPingProgress + 1.0f, fPingProgress + 1.0f);
            ping.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - Mathf.Abs(fPingProgress - 1.0f));
            float fAngle = Mathf.Atan2(deltaPos.z, deltaPos.x) * Mathf.Rad2Deg;
            ping.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, fAngle - 90.0f);
        }
        else
        {
            ping.enabled = false;
        }
    }

    // Population functions
    public void PopulateCooldownImage_SpaceshipPart0(Image image)
    {
        image.fillAmount = Core.GetCore().theSpaceship.m_xParts[0].m_bPurchased ? 1.0f : 0.0f;
    }
    public void PopulateCooldownImage_SpaceshipPart1(Image image)
    {
        image.fillAmount = Core.GetCore().theSpaceship.m_xParts[1].m_bPurchased ? 1.0f : 0.0f;
    }
    public void PopulateCooldownImage_SpaceshipPart2(Image image)
    {
        image.fillAmount = Core.GetCore().theSpaceship.m_xParts[2].m_bPurchased ? 1.0f : 0.0f;
    }
    public void PopulateCooldownImage_SpaceshipPart3(Image image)
    {
        image.fillAmount = Core.GetCore().theSpaceship.m_xParts[3].m_bPurchased ? 1.0f : 0.0f;
    }
    public void PopulateCooldownImage_SpaceshipPart4(Image image)
    {
        image.fillAmount = Core.GetCore().theSpaceship.m_xParts[4].m_bPurchased ? 1.0f : 0.0f;
    }

    public void PopulateCooldownImage_JetPack(Image image)
    {
        float filledAmount = GetCooldownFilledAmount(PlayerBehaviour.Skill.JETPACK);
        image.fillAmount = filledAmount;
    }

    public void PopulateCooldownImage_Scanner(Image image)
    {
        float filledAmount = GetCooldownFilledAmount(PlayerBehaviour.Skill.PING);
        image.fillAmount = filledAmount;
    }

    public void PopulateCooldownImage_Stun(Image image)
    {
        float filledAmount = GetCooldownFilledAmount(PlayerBehaviour.Skill.STUN);
        image.fillAmount = filledAmount;
    }

    public void PopulateCooldownImage_SlowEnemy(Image image)
    {
        float filledAmount = GetCooldownFilledAmount(PlayerBehaviour.Skill.SLOW);
        image.fillAmount = filledAmount;
    }

    public void PopulateCooldownImage_SpeedBoost(Image image)
    {
        float filledAmount = GetCooldownFilledAmount(PlayerBehaviour.Skill.SPEED_BOOST);
        image.fillAmount = filledAmount;
    }

    float GetCooldownFilledAmount(PlayerBehaviour.Skill skill)
    {
        float filledAmount = 0.0f;

        PlayerBehaviour.SkillData skillData = Core.GetCore().thePlayer.skills[(int)skill];
        if (!skillData.bUnlocked)
        {
            filledAmount = 0.0f;
        }
        else
        {
            if (skillData.fCooldown == 0.0f)
            {
                filledAmount = 1.0f;
            }
            else
            {
                filledAmount = skillData.fTimeSinceUsed / skillData.fCooldown;
                if (filledAmount > 1.0f)
                {
                    filledAmount = 1.0f;
                }
                else if (filledAmount < 0.0f)
                {
                    filledAmount = 0.0f;
                }
            }
        }

        return filledAmount;
    }

    public void PopulateCarryingCapacity(Text text)
    {
        int iCollectedResources = Core.GetCore().thePlayer.collector.collectedResources.Count;
        int iCapacity = Core.GetCore().thePlayer.collector.capacity;

        text.text = iCollectedResources + " / " + iCapacity;
    }

    public void PopulateEnemySpeed(Text text)
    {
        int iCollectedResources = Core.GetCore().thePlayer.collector.collectedResources.Count;

        float fSpeed = (iCollectedResources + 1);

        text.text = "Enemy Speed: " + fSpeed;
    }

    // Visibility conditions
    public void ShouldPopulateButtonPrompt_JetPack(Text text)
    {
        text.enabled = ShouldPopulateButtonPrompt(PlayerBehaviour.Skill.JETPACK);
    }

    public void ShouldPopulateButtonPrompt_Scanner(Text text)
    {
        text.enabled = ShouldPopulateButtonPrompt(PlayerBehaviour.Skill.PING);
    }

    public void ShouldPopulateButtonPrompt_Stun(Text text)
    {
        text.enabled = ShouldPopulateButtonPrompt(PlayerBehaviour.Skill.STUN);
    }

    public void ShouldPopulateButtonPrompt_SlowEnemy(Text text)
    {
        text.enabled = ShouldPopulateButtonPrompt(PlayerBehaviour.Skill.SLOW);
    }

    public void ShouldPopulateButtonPrompt_SpeedBoost(Text text)
    {
        text.enabled = ShouldPopulateButtonPrompt(PlayerBehaviour.Skill.SPEED_BOOST);
    }

    bool ShouldPopulateButtonPrompt(PlayerBehaviour.Skill skill)
    {
        PlayerBehaviour.SkillData skillData = Core.GetCore().thePlayer.skills[(int)skill];
        return skillData.bUnlocked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}