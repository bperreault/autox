package autox.core.base;

public interface Data {
	void Create(String id, Object data);
	Object Read(String id);
	void Update(String id, Object data);
	void Delete(String id);
}
