public class Constants
{
  /* 
   * ROOM PROPERTIES
   */
  public const float ROOM_LENGTH = 2500f;
  public const float ROOM_WIDTH = 2000f;

  /* 
   * AI BEHAVIOUR PROPERTIES
   */
  public const float AI_POSITION_LERP_SPEED = 0.1f;
  public const float AI_MAX_AVOID_PERCENTAGE = 0.7f;

  /* 
  * FLARE NAMES
  */
  public const string FLARE_NAME_PARTICLE_SPAWN_POSITION = "ParticleSpawnPosition";
  public const string FLARE_NAME_ROTATION_SPEED = "RotationSpeed";
  public const string FLARE_NAME_ATTRACTIVE_STRENGTH = "AttractiveStrength";
  public const string FLARE_NAME_PARTICLE_SPAWN_RATE = "ParticleSpawnRate";
  public const string FLARE_NAME_LIFETIME = "Lifetime";
  public const string FLARE_NAME_PARTICLE_GRADIENT = "ParticleColourGradient";

  /* 
   * FLARE ATTRIBUTES
   */
  public const float FLARE_PARTICLE_AVOID_MODIFIER = 0.75f;
  public const float FLARE_MIN_ATTRACTIVE_STRENGTH = 3f;
  public const float FLARE_MAX_ATTRACTIVE_STRENGTH = 150f;
  public const float FLARE_MIN_ROTATION_SPEED = 15f;
  public const float FLARE_MAX_ROTATION_SPEED = 1f;
  public const float FLARE_MIN_PARTICLE_SPAWN_RATE = 100f;
  public const float FLARE_MAX_PARTICLE_SPAWN_RATE = 1200f;
  public const float FLARE_MIN_LIFETIME = 14f;
  public const float FLARE_MAX_LIFETIME = 1f;
  public const float FLARE_ALARMED_MIN_ATTRACTIVE_STRENGTH_MULTIPLIER = 1.5f;
  public const float FLARE_ALARMED_MAX_ATTRACTIVE_STRENGTH_MULTIPLIER = 3.5f;

  /* 
   * ATTRACTOR ATTRIBUTES
   */
  public const float ATTRACTOR_MIN_SPEED = 1f;
  public const float ATTRACTOR_MAX_SPEED = 8f;
}