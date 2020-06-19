using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacks : MonoBehaviour
{
    private MyTPCharacter tpc;
    private float minTimeBetweenPunches = 1;
    private float timeSinceLastPunch = 0;
    private int punchCount = 0;
    private bool canPunch = true;
    private CameraMovement _cm;
    private Transform enemy;
    private Vector3 enemyDir;
    public List<PunchAnimation> punches;
    
    [System.Serializable]
    public struct PunchAnimation{
        [Tooltip("Seconds that the player has to press action button again to chain this animation with the next")]
        public float chainTime;

        [Tooltip("Instant (in seconds from the beginning) in which the punch hits.")]
        public float hitTime;

    }

    // Start is called before the first frame update
    void Start()
    {
        minTimeBetweenPunches = GetMaxHitTime();
        tpc = FindObjectOfType<MyTPCharacter>();
        _cm = GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentEnemy();   // Change the current enemy we are going to attack if necesary

        timeSinceLastPunch += Time.deltaTime;
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);   // Calculate distance to enemy
        
        if (Input.GetKeyDown(KeyCode.E) && canPunch && distanceToEnemy < 5.0f)  // If we press 'E' key and are close enough to the targeted enemy
        {
            timeSinceLastPunch = 0;
            punchCount++;

            canPunch = false;
            Invoke("AllowPunch", minTimeBetweenPunches);    // Allows punch again in 'minTimeBetweenPunches' seconds
            Invoke("DelayedEnemyHit", punches[punchCount-1].hitTime); // Activate the enemy hit in the frame the animation finish the punch
            
            string punchAnimName = "Punch" + punchCount;
            tpc.GetFullBodyAnimator().speed = 1.0f; // Change this in case we want to accelerate punch animations
            tpc.GetFullBodyAnimator().Play(punchAnimName);  

            if (punchCount == 1) {  // The first punch requires some configurations
                // Rotate both characters so they face each other
                tpc.transform.LookAt(enemy);
                enemy.LookAt(transform);

                // Move the camera to a position where you can see the action happening
                _cm.TransitionTo(_cm.aimCam.position, _cm.aimCam.rotation.eulerAngles, enemy.GetComponent<Enemy>().GetLookAt(), 1.75f);
                
                // Locks camera and player movement so you are always looking action sequence
                _cm.LockCamera();
                GetComponent<Player>().LockMovement();
            }
        }

        // Approaches the enemy while playing first punch animation
        if (punchCount == 1 && distanceToEnemy < 5.0f) {
            ApproachEnemy();
        }

        // Checks if the punch chain has been interrupted
        CheckPunchChain();
    }


    void CheckPunchChain() {
        if (punchCount > 0 &&
            (punchCount >= punches.Count || timeSinceLastPunch >= punches[punchCount - 1].chainTime)) {
            punchCount = 0;
            tpc.GetFullBodyAnimator().speed = 1.0f;
            _cm.BackToStart();
            GetComponent<Player>().UnlockMovement();
        }
    }

    void AllowPunch() { canPunch = true; }

    void ApproachEnemy() {
        enemyDir = transform.position - enemy.position;
        enemyDir = Vector3.Scale(enemyDir, new Vector3(1, 0, 1)).normalized;
        Vector3 targetPos = enemy.position + enemyDir * 2.0f;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.25f);

    }

    private void DelayedEnemyHit() {
        enemy.GetComponent<Enemy>().Hit();
        
    }

    private void UpdateCurrentEnemy() {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Vector3 playerFwd = Vector3.Scale(tpc.transform.forward, new Vector3(1, 0, 1)).normalized;

        float minAngle = 360.0f;

        foreach (Enemy e in enemies) {
            Vector3 direction = e.transform.position - transform.position;
            float angle = Vector3.Angle(playerFwd, direction);

            if (angle < minAngle) {
                enemy = e.transform;
                minAngle = angle;
            }
        }

    }

    private float GetMaxHitTime() {
        float currentMax = 0;

        foreach (PunchAnimation pa in punches) {
            if (pa.hitTime > currentMax) currentMax = pa.hitTime;
        }

        return currentMax;
    }

}
