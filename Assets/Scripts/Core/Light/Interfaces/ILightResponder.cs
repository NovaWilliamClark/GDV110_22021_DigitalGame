namespace Core
{
    public interface ILightResponder
    {
        public void OnLightEntered(float intensity);
        public void OnLightExited(float intensity);
        public void OnLightStay(float intensity);
    }
}