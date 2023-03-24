using Matryoshka.Abilities;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectController;
using Matryoshka.Game;

public class BulletSpawner : MonoBehaviour
{

    public ProjectileEffect projectileEffect;
    public ProjectileEffect projectileEffectAngry;
    public ProjectileEffect projectileEffectAngry2;
    public GameObject projectilePrefab;
    private bool isFiringPattern = false;
    private int offset = 0;
    public int positionOffset = 180;
    Vector2 blank = new Vector2(0,0);
    private int cycleCount = 0;
    private int cycleCountA = 0;
    private bool isAngry = false;
 
    public void FireRandomPattern(){
        isFiringPattern = true;
        int randomNumber = UnityEngine.Random.Range(0, 6);
        if(randomNumber == 0){
            for(int i = 0; i<10; i++){
                StartCoroutine("ArcPattern",i);
            }
        }
        if(randomNumber == 1){
            for(int i = 0; i< 20; i++){
                StartCoroutine("Spiral", i);
            }
        }
        if(randomNumber == 2){
            for(int i = 0; i< 36; i++){
                StartCoroutine("CloseIn", i);
            }
        }
        if(randomNumber == 3){
            for(int i = 0; i< 36; i++){
            StartCoroutine("SpreadOut", i);
            }
        }
        if(randomNumber == 4){
            for(int i = 0; i< 36; i++){
                StartCoroutine("CloseIn", i);
                StartCoroutine("SpreadOut", i);
            }
        }
        if(randomNumber == 5){
            for(int i = 0; i< 15; i++){
                StartCoroutine("Burst", i);
            }
        }
    }

    //Anger Phase will last 9 seconds and then end by itself
    public void BeginAngerPhase(){
        if(!isAngry)
            {
                isAngry = true;
                StartCoroutine("ArcPatternAngry", 0.5f);
                // StartCoroutine("BurstAngry", 0.75f);
                // AngrySpread();
            }        
    }

    private void AngrySpread() {
        for(int i = 0; i< 36; i++){
            StartCoroutine("SpreadAngry", i);
        }
    }


    private IEnumerator ArcPattern(float t){
        
        yield return new WaitForSeconds(t*0.75f);
        float angleStep = 18f;
        float angle = -90f + offset;
        angle += positionOffset;

        for(int i = 0; i<11; i++){
            float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 direction = new Vector3(dirX, dirY, 0);
        
            Spawn(direction);
            
            angle += angleStep;
        }
        if(offset == 0){offset = 9;}
        else{offset =0;}
        cycleCount ++;
        if(cycleCount == 10){
            yield return new WaitForSeconds(1.5f);
            isFiringPattern = false; 
            cycleCount=0;
        }
        
    }

    private IEnumerator Spiral(float t){
        
        yield return new WaitForSeconds(t*0.375f);
        float angle = -90+(t*9);
        angle += positionOffset;
        float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
        float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 direction = new Vector3(dirX, dirY, 0);

        Spawn(direction);
        cycleCount ++;
        if(cycleCount == 20){
            yield return new WaitForSeconds(1.5f);
            isFiringPattern = false;  
            cycleCount=0;
        }
    }

    private IEnumerator CloseIn(float t){
        
        yield return new WaitForSeconds(t*0.25f);
        float angle = 0;
        
        if(cycleCount %2 == 0){
            angle = 270+(t*2.5f);
            angle += positionOffset;
        }
        else{
            angle = 90-(t*2.5f);
            angle += positionOffset;
        }
        
        float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
        float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 direction = new Vector3(dirX, dirY, 0);

        Spawn(direction);
        cycleCount ++;
        if(cycleCount == 36){
            yield return new WaitForSeconds(1.5f);
            isFiringPattern = false;  
            cycleCount=0;
        }

    }

    private IEnumerator SpreadOut(float t){
        
        yield return new WaitForSeconds(t*0.25f);
        float angle = 0;
        
        if(cycleCountA %2 == 0){
            angle = 0+(t * 2.5f);
            angle += positionOffset;
        }
        else{
            angle = 0-(t * 2.5f);
            angle += positionOffset;
        }
        
        float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
        float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 direction = new Vector3(dirX, dirY, 0);

        Spawn(direction);
        cycleCountA ++;
        if(cycleCountA == 36){
            yield return new WaitForSeconds(1.5f);
            isFiringPattern = false;
            cycleCountA=0;
        }
    }

    private IEnumerator Burst(float t){
        
        yield return new WaitForSeconds(t*0.5f);

        float angle = UnityEngine.Random.Range(-90, 90);
        angle += positionOffset;

        float dirX =  Mathf.Sin((angle * Mathf.PI) / 180f);
        float dirY =  Mathf.Cos((angle * Mathf.PI) / 180f);

        Vector3 direction = new Vector3(dirX, dirY, 0);
        Spawn(direction);
        
        yield return new WaitForSeconds(0.05f);
        for(int i = 1; i < 3; i++){
            float angle1 = angle + (i*2);
            float angle2 = angle - (i*2);

            float dirX1 = Mathf.Sin((angle1 * Mathf.PI) / 180f);
            float dirY1 = Mathf.Cos((angle1 * Mathf.PI) / 180f);

            float dirX2 = Mathf.Sin((angle2 * Mathf.PI) / 180f);
            float dirY2 = Mathf.Cos((angle2 * Mathf.PI) / 180f);

            Vector3 direction1 = new Vector3(dirX1, dirY1, 0);
            Vector3 direction2 = new Vector3(dirX2, dirY2, 0);

            Spawn(direction1);
            Spawn(direction2);
            yield return new WaitForSeconds(0.05f);
        }
        cycleCount ++;
        if(cycleCount == 15){
            yield return new WaitForSeconds(1.5f);
            isFiringPattern = false;  
            cycleCount=0;
        }

    }

    public void Spawn(Vector2 direction)
    {
        NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
            projectilePrefab,
            transform.position,
            new Quaternion(0,0,0,0)
            );
        projectileObject.Spawn(true);
        Projectile projectile = projectileObject.gameObject.GetComponent<Projectile>();
        projectile.Fire(projectileEffect, direction);
        
        
    }


    // Spawn Angry and Spawn Angry 2 are currently broken, will fix later time permitting
    public void SpawnAngry(Vector2 direction)
    {
        NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
            projectilePrefab,
            transform.position,
            new Quaternion(0,0,0,0)
            );
        projectileObject.Spawn(true);
        Projectile projectile = projectileObject.gameObject.GetComponent<Projectile>();
        projectile.Fire(projectileEffectAngry, direction);
    }

    public void SpawnAngry2(Vector2 direction)
    {
        NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
            projectilePrefab,
            transform.position,
            new Quaternion(0,0,0,0)
            );
        projectileObject.Spawn(true);
        Projectile projectile = projectileObject.gameObject.GetComponent<Projectile>();
        projectile.Fire(projectileEffectAngry2, direction);
    }


    //set offset based on Mittens position
    // 0 = top, 1 = right, 2 = bottom, 3 = left
    public void setMittensPosition(int position){
        // Top
        if(position == 0){
            positionOffset = 180;
        }
        // Right
        if(position == 1){
            positionOffset = 270;
        }
        // Bottom
        if(position == 2){
            positionOffset = 0;
        }
        // Left
        if(position == 3){
            positionOffset = 90;
        }
    }

    private IEnumerator ArcPatternAngry(float pause){

        for(int i =0; i < 18; i++){
            yield return new WaitForSeconds(pause);

            float angleStep = 19f;
            float angle = -90f + offset;
            angle += positionOffset;

            for(int j = 0; j<10; j++){
                float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
                float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

                Vector3 direction = new Vector3(dirX, dirY, 0);
        
                Spawn(direction);
            
                angle += angleStep;
            }
            if(offset == 0){offset = 9;}
            else{offset =0;}
        }
        yield return new WaitForSeconds(1);
        isAngry = false;
    }

    private IEnumerator BurstAngry(float pause){

        for(int i =0; i < 12; i++){
            yield return new WaitForSeconds(pause);
        
            float angle = UnityEngine.Random.Range(-90, 90);
            angle += positionOffset;

            float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 direction = new Vector3(dirX, dirY, 0);
            SpawnAngry2(direction);
        
        
            for(int j = 1; j < 3; j++){
                float angle1 = angle + (j*2);
                float angle2 = angle - (j*2);

                float dirX1 = Mathf.Sin((angle1 * Mathf.PI) / 180f);
                float dirY1 = Mathf.Cos((angle1 * Mathf.PI) / 180f);

                float dirX2 = Mathf.Sin((angle2 * Mathf.PI) / 180f);
                float dirY2 = Mathf.Cos((angle2 * Mathf.PI) / 180f);

                Vector3 direction1 = new Vector3(dirX1, dirY1, 0);
                Vector3 direction2 = new Vector3(dirX2, dirY2, 0);

                SpawnAngry2(direction1);
                SpawnAngry2(direction2);
            
            }
        }
    }

    private IEnumerator SpreadAngry(float t){
        
        yield return new WaitForSeconds(t*0.25f);
        float angle = 0;
        float angle2 = 0;
        
        if(cycleCount %2 == 0){
            angle = 270+(t*2.5f);
            angle += positionOffset;

            angle2 = 0+(t * 2.5f);
            angle2 += positionOffset;
        }
        else{
            angle = 90-(t*2.5f);
            angle += positionOffset;

            angle2 = 0-(t * 2.5f);
            angle2 += positionOffset;
        }
        
        float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
        float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

        float dirX2 = Mathf.Sin((angle2 * Mathf.PI) / 180f);
        float dirY2 = Mathf.Cos((angle2 * Mathf.PI) / 180f);

        Vector3 direction = new Vector3(dirX, dirY, 0);
        Vector3 direction2 = new Vector3(dirX2, dirY2, 0);

        SpawnAngry(direction);
        SpawnAngry(direction2);
        cycleCount ++;
        if(cycleCount == 36){
            cycleCount=0;
        }
    }

    public bool IsFiringPattern(){
        return isFiringPattern;
    }
}
