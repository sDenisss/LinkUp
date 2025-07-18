namespace LinkUp.Domain
{
    public enum MessageStatus
    {
        Sent,    // Отправлено
        Delivered, // Доставлено получателю
        Read,    // Прочитано
        Edited,  // Отредактировано
        Deleted  // Удалено (мягкое удаление)
    }
}