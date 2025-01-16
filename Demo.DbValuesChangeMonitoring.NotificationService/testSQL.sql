update
configuration.ConfigurationValues
set
[Value] = 'Medium'
where
[Key] = 'Display:FontSize'

select 
*
from
configuration.ConfigurationValues

SELECT 
    conversation_handle,
    message_type_name,
    CAST(message_body AS NVARCHAR(MAX)) AS message_body_text,
    service_name,
    GETDATE() AS peek_time
FROM 
    ValuesChangeEventQueue;
