using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    public enum Skill
    {
        JETPACK,
        PING,
        STUN,
        SLOW,
        SPEED_BOOST
    }
    public static readonly int iNUM_SKILLS = 5;

    [System.Serializable]
    public class SkillData
    {
        // Prefab
        public float fCooldown = 1.0f;
        public string inputAxis = "";
        public AudioClip soundEffect = null;

        // Runtime
        public float fTimeSinceUsed = 0.0f;
        public bool bUnlocked = false;
        public bool bCanUse = false;
    }

    public SkillData[] skills = new SkillData[iNUM_SKILLS];

    public float interactDistance = 4.0F;
    public float jumpSpeed = 8.0F;

    public ResourceCollector collector;

	public float speed = 1.0F;
    public float fSpeedBoostActive = 0.0f;

    public float currentYSpeed = 0.0F;

    public Transform body, head;
    public float fBodyAngle = 0.0f, fHeadAngle = 0.0f;
    public Transform followPoint;

    public IInteractable hoveringOver = null;

    public MeshRenderer slowPFX;
    public float fSlowPFXTime = 0.0f;
    public float fStunPFXTime = 0.0f;

    // Use this for initialization
    void Start () {
        collector = GetComponentInChildren<ResourceCollector>();
        skills[(int)Skill.JETPACK].bUnlocked = true;

        // --- DEBUG
        for (int i = 0; i < 5; i++)
            skills[i].bUnlocked = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool CanUseSkill(Skill skill)
    {
        if (!skills[(int)skill].bUnlocked)
            return false;

        switch(skill)
        {
            case Skill.JETPACK:
                return GetComponent<CharacterController>().isGrounded;
            case Skill.STUN:
                return hoveringOver != null && hoveringOver.GetGameObject().GetComponent<BasicEnemy>() != null;
            case Skill.PING:
            case Skill.SLOW:
            case Skill.SPEED_BOOST:
                return true;
        }

        return false;
    }

    private void Ping()
    {
        ResourceDeposit bestDeposit = null;
        float fBestDistance = 10000.0f;
        foreach (ResourceDeposit deposit in ResourceDeposit.deposits)
        {
            float fDistance = (deposit.transform.position - transform.position).magnitude;
            if (fDistance < fBestDistance)
            {
                bestDeposit = deposit;
                fBestDistance = fDistance;
            }
        }
        if (bestDeposit != null)
        {
            Core.GetCore().theHUD.pingTarget = bestDeposit.transform.position;
            Core.GetCore().theHUD.fPingProgress = 0.0f;
        }
        else
        {
            // TODO : Make a "nah m8" sound effect
        }
    }

    private void Stun()
    {
        BasicEnemy target = hoveringOver.GetGameObject().GetComponent<BasicEnemy>();
        target.Stun(5.0f);

        LineRenderer lr = GetComponent<LineRenderer>();
        List<Vector3> positions = new List<Vector3>();

        positions.Add(transform.position);

        Vector3 dPos = target.transform.position - transform.position;
        float fDistance = dPos.magnitude;
        dPos.Normalize();
        int iNumSteps = 0;

        while (fDistance > 1.0f)
        {
            iNumSteps++;
            fDistance -= 1.0f;
            Vector3 pos = transform.position + dPos * iNumSteps + Random.onUnitSphere * 0.5f;
            positions.Add(pos);
        }

        positions.Add(target.transform.position);

        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
        lr.enabled = true;
        fStunPFXTime = 0.1f;
    }

    private void Slow()
    {
        fSlowPFXTime = 0.0f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10.0f);
        foreach(Collider collider in colliders)
        {
            BasicEnemy enemy = collider.GetComponent<BasicEnemy>();
            if (enemy != null)
            {
                enemy.Slow(2.0f);
            }
        }
    }

	void FixedUpdate ()
    {
        CharacterController controller = GetComponent<CharacterController>();
        Vector3 horizontalVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * speed * (fSpeedBoostActive > 0.0f ? 2.0f : 1.0f);
        Vector3 velocity = controller.velocity;

        fSpeedBoostActive -= Time.deltaTime;

        if(fSlowPFXTime < 2.0f)
        {
            fSlowPFXTime += Time.deltaTime;
            slowPFX.enabled = true;
            slowPFX.transform.localEulerAngles = new Vector3(0.0f, -360.0f * fSlowPFXTime, 0.0f);
            slowPFX.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - Mathf.Abs(fSlowPFXTime - 1.0f));
        }
        else
            slowPFX.enabled = false;

        if (fStunPFXTime > 0.0f)
        {
            fStunPFXTime -= Time.deltaTime;

            if (fStunPFXTime <= 0.0f)
            {
                GetComponent<LineRenderer>().enabled = false;
            }
        }

        for (int i = 0; i < iNUM_SKILLS; i++)
        {
            skills[i].bCanUse = CanUseSkill((Skill)i);
            skills[i].fTimeSinceUsed += Time.deltaTime;
            if (skills[i].fTimeSinceUsed < skills[i].fCooldown)
                skills[i].bCanUse = false;

            if(skills[i].bCanUse && Input.GetAxis(skills[i].inputAxis) > 0.0f)
            {
                switch(i)
                {
                    case (int)Skill.JETPACK:
                    {
                        if (velocity.y < 0)
                        {
                            velocity.y = 0;
                        }
                        velocity.y += jumpSpeed;
                        break;
                    }
                    case (int)Skill.PING:
                    {
                        Ping();
                        break;
                    }
                    case (int)Skill.SLOW:
                    {
                        Slow();
                        break;
                    }
                    case (int)Skill.STUN:
                    {
                        Stun();
                        break;
                    }
                    case (int)Skill.SPEED_BOOST:
                    {
                        fSpeedBoostActive = 1.0f;
                        break;
                    }
                }

                //skills[i].soundEffect;
                skills[i].fTimeSinceUsed = 0.0f;
            }
        }

        velocity += Physics.gravity * Time.fixedDeltaTime;

        if (velocity.magnitude > 1.0f)
        {
            float fAngle = Mathf.Rad2Deg * Mathf.Atan2(velocity.z, velocity.x);
            fBodyAngle = Mathf.LerpAngle(fBodyAngle, fAngle, 8.0f * Time.fixedDeltaTime);
            body.localEulerAngles = new Vector3(0.0f, 180.0f - fBodyAngle, 0.0f);
        }
         
        fHeadAngle = Mathf.LerpAngle(fHeadAngle, fBodyAngle, 4.0f * Time.fixedDeltaTime);
        head.localEulerAngles = new Vector3(0.0f, 180.0f - fHeadAngle, 0.0f);

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        controller.Move(velocity * Time.fixedDeltaTime);

        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool bInRange = false;
        if (Physics.Raycast(ray, out hit) && hit.collider != null)
        {
            IInteractable newHover = hit.collider.GetComponentInParent<IInteractable>();
            if (newHover != null)
            {
                bInRange = (hit.collider.transform.position - transform.position).magnitude < interactDistance;

                if (hoveringOver != null && newHover != hoveringOver)
                {
                    hoveringOver.Unhover();
                }

                hoveringOver = newHover;
            }
            else
            {
                if (hoveringOver != null)
                {
                    hoveringOver.Unhover();
                    hoveringOver = null;
                }
            }
        }
        else
        {
            if (hoveringOver != null)
            {
                hoveringOver.Unhover();
                hoveringOver = null;
            }
        }

        if(hoveringOver != null)
        {
            hoveringOver.Hover(bInRange);

            if (bInRange)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    hoveringOver.Unhover();
                    hoveringOver.Interact(this, 0);
                    hoveringOver = null;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    hoveringOver.Unhover();
                    hoveringOver.Interact(this, 1);
                    hoveringOver = null;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    // Play a nastay SFX
                }
            }
        }
	}


    

    public void UnlockSkill(int iSkillIndex)
    {
        if( iSkillIndex < iNUM_SKILLS)
        {
            SkillData xSkill = skills[iSkillIndex];
            xSkill.bUnlocked = true;
        }
    }
}
