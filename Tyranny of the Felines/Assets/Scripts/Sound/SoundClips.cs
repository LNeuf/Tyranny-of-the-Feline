using System.Collections.Generic;
using UnityEngine;

public class SoundClips : Object
{
    // Music
    public static AudioClip BossMusic = Resources.Load<AudioClip>("Sound/Music/BossTheme");
    public static AudioClip MenuMusic = Resources.Load<AudioClip>("Sound/Music/MenuTheme");
    public static AudioClip CharacterMusic = Resources.Load<AudioClip>("Sound/Music/CharacterSelectTheme");
    
    // Big sound effects
    public static AudioClip BigDeath = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigDeath");
    public static AudioClip BigOverheadSlash1 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigOverheadSlash1");
    public static AudioClip BigOverheadSlash2 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigOverheadSlash2");
    public static AudioClip BigShieldWall1 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigShieldWall1");
    public static AudioClip BigShieldWall2 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigShieldWall2");
    public static AudioClip BigShieldWall3 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigShieldWall3");
    public static AudioClip BigSlash1 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigSlash1");
    public static AudioClip BigSlash2 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigSlash2");
    public static AudioClip BigSlash3 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigSlash3");
    public static AudioClip BigWalk1 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk1");
    public static AudioClip BigWalk2 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk2");
    public static AudioClip BigWalk3 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk3");
    public static AudioClip BigWalk4 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk4");
    public static AudioClip BigWalk5 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk5");
    public static AudioClip BigWalk6 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk6");
    public static AudioClip BigWalk7 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk7");
    public static AudioClip BigWalk8 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk8");
    public static AudioClip BigWalk9 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk9");
    public static AudioClip BigWalk10 = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWalk10");
    public static AudioClip BigWhistle = Resources.Load<AudioClip>("Sound/SoundEffects/big/BigWhistle");
    
    // furball sound effects
    public static AudioClip Furball1 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball1");
    public static AudioClip Furball2 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball2");
    public static AudioClip Furball3 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball3");
    public static AudioClip Furball4 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball4");
    public static AudioClip Furball5 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball5");
    public static AudioClip Furball6 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball6");
    public static AudioClip Furball7 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball7");
    public static AudioClip Furball8 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball8");
    public static AudioClip Furball9 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball9");
    public static AudioClip Furball10 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball10");
    public static AudioClip Furball11 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball11");
    public static AudioClip Furball12 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball12");
    public static AudioClip Furball13 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball13");
    public static AudioClip Furball14 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball14");
    public static AudioClip Furball15 = Resources.Load<AudioClip>("Sound/SoundEffects/furball/furball15");
    
    // menu sound effects
    public static AudioClip Click1 = Resources.Load<AudioClip>("Sound/SoundEffects/menu/Click1");
    public static AudioClip Click2 = Resources.Load<AudioClip>("Sound/SoundEffects/menu/Click2");
    public static AudioClip Click3 = Resources.Load<AudioClip>("Sound/SoundEffects/menu/Click3");
    public static AudioClip Click4 = Resources.Load<AudioClip>("Sound/SoundEffects/menu/Click4");
    public static AudioClip Click5 = Resources.Load<AudioClip>("Sound/SoundEffects/menu/Click5");

    // Mittens sound effects
    public static AudioClip MittensMeow = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensMeow");
    public static AudioClip MittensClawSwipe = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensClawSwipe");
    public static AudioClip MittensTailSwipe = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensTailSwipe");
    public static AudioClip MittensSpit = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensSpit");
    public static AudioClip MittensImpale = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensImpale");
    public static AudioClip MittensScream = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensScream");
    public static AudioClip MittensFakeDeath = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensFakeDeath");
    public static AudioClip MittensTeleport = Resources.Load<AudioClip>("Sound/SoundEffects/mittens/MittensTeleport");
    
    // Salumon sound effects
    public static AudioClip SalumonDeath = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonDeath");
    public static AudioClip SalumonBubbles = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonBubbles");
    public static AudioClip SalumonWalking1 = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonWalk1");
    public static AudioClip SalumonWalking2 = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonWalk2");
    public static AudioClip SalumonHeal = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonHeal");
    public static AudioClip SalumonMagicMissile = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonMagicMissile");
    public static AudioClip SalumonBigHeal = Resources.Load<AudioClip>("Sound/SoundEffects/salumon/SalumonBigHeal");
    
    // tower sound effects
    public static AudioClip TowerDamaged1 = Resources.Load<AudioClip>("Sound/SoundEffects/tower/TowerDamaged1");
    public static AudioClip TowerDamaged2 = Resources.Load<AudioClip>("Sound/SoundEffects/tower/TowerDamaged2");
    public static AudioClip TowerDamaged3 = Resources.Load<AudioClip>("Sound/SoundEffects/tower/TowerDamaged3");
    
    // wazo sound effects
    public static AudioClip WazoDeath = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoDeath");
    public static AudioClip WazoWhistle = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoWhistle");
    public static AudioClip WazoWalking = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoWalk");
    public static AudioClip WazoDash = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoDash");
    public static AudioClip WazoFeatherThrow = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoFeatherThrow");
    public static AudioClip WazoHailOfFeathers = Resources.Load<AudioClip>("Sound/SoundEffects/wazo/WazoHailOfFeathers");
}
