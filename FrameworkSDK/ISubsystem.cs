using System;

namespace FrameworkSDK
{
    /// <summary>
    /// "Подпрограмма" независимая от конкретного приложения. Реагирует на определенные события со стороны абстрактного приложения.
    /// Не может зависеть от каких либо сервисов, но сама может регистрировать сервисы для взаимодействия с собой.
    /// </summary>
	public interface ISubsystem : INamed, IDisposable
	{
		void Initialize();

		void OnGameActivated(Application application);

		void OnGameDeactivated(Application application);

		void OnGameEnding(Application application);
	}
}
