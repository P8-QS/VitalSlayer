
public class EmailSetEvent: Unity.Services.Analytics.Event
{
	public EmailSetEvent() : base("emailSetEvent")
	{
	}

	public string Email { set { SetParameter("email", value); } }
}