using System.ComponentModel.DataAnnotations;

namespace Fab.Infrastructure.Interfaces.Sms;

public enum ResponseCode
{
    [Display(Description = "Сообщение не найдено")]
    MessageNotFound = -1,

    [Display(Description = "Запрос выполнен или сообщение находится в нашей очереди")]
    Completed = 100,

    [Display(Description = "Сообщение передается оператору")]
    MessageSending = 101,

    [Display(Description = "Сообщение отправлено (в пути)")]
    MessageSent = 102,

    [Display(Description = "Сообщение доставлено")]
    MessageDelivered = 103,

    [Display(Description = "Не может быть доставлено: время жизни истекло")]
    MessageTtlExpired = 104,

    [Display(Description = "Не может быть доставлено: удалено оператором")]
    MessageDeleted = 105,

    [Display(Description = "Не может быть доставлено: сбой в телефоне")]
    MessagePhoneDeliveryError = 106,

    [Display(Description = "Не может быть доставлено: неизвестная причина")]
    MessageUnknownDeliveryError = 107,

    [Display(Description = "Не может быть доставлено: отклонено")]
    MessageRejected = 108,

    [Display(Description = "Сообщение прочитано")]
    MessageRead = 110,

    [Display(Description = "Не может быть доставлено: не найден маршрут на данный номер")]
    MessageDeliveryError = 150,

    [Display(Description = "Неправильный api_id")]
    WrongApiKey = 200,

    [Display(Description = "Не хватает средств на лицевом счету")]
    NotEnoughMoney = 201,

    [Display(Description = "Неправильно указан номер телефона получателя, либо на него нет маршрута")]
    WrongRecipient = 202,

    [Display(Description = "Нет текста сообщения")]
    MessageWithoutText = 203,

    [Display(Description = "Имя отправителя не согласовано с администрацией")]
    WrongSender = 204,

    [Display(Description = "Сообщение слишком длинное (превышает 8 СМС)")]
    MessageTooLong = 205,

    [Display(Description = "Будет превышен или уже превышен дневной лимит на отправку сообщений")]
    MessageDailyLimitExceeded = 206,

    [Display(Description = "На этот номер нет маршрута для доставки сообщений")]
    RecipientUnableToDeliverMessage = 207,

    [Display(Description = "Параметр time указан неправильно")]
    WrongTime = 208,

    [Display(Description = "Вы добавили этот номер (или один из номеров) в стоп-лист")]
    RecipientInStopList = 209,

    [Display(Description = "Используется GET, где необходимо использовать POST")]
    WrongHttpMethod = 210,

    [Display(Description = "Метод не найден")]
    MethodNotFound = 211,

    [Display(Description =
        "Текст сообщения необходимо передать в кодировке UTF-8 (вы передали в другой кодировке)")]
    MessageWrongEncoding = 212,

    [Display(Description = "Указано более 100 номеров в списке получателей")]
    MessageTooManyRecipients = 213,

    [Display(Description = "Сервис временно недоступен, попробуйте чуть позже")]
    ServiceUnavailable = 220,

    [Display(Description = "Превышен общий лимит количества сообщений на этот номер в день")]
    RecipientDailyLimitExceeded = 230,

    [Display(Description = "Превышен лимит одинаковых сообщений на этот номер в минуту")]
    SimilarMessageMinuteLimitExceeded = 231,

    [Display(Description = "Превышен лимит одинаковых сообщений на этот номер в день")]
    SimilarMessageDailyLimitExceeded = 232,

    [Display(Description =
        "Превышен лимит отправки повторных сообщений с кодом на этот номер за короткий промежуток времени")]
    MessageResendLimitExceeded = 233,

    [Display(Description = "Неправильный token")]
    WrongToken = 300,

    [Display(Description = "Неправильный api_id, либо логин/пароль")]
    WrongApiKeyOrCredentials = 301,

    [Display(Description = "Пользователь авторизован, но аккаунт не подтвержден")]
    AccountNotConfirmed = 302,

    [Display(Description = "Код подтверждения неверен")]
    WrongConfirmationCode = 303,

    [Display(Description = "Отправлено слишком много кодов подтверждения. Пожалуйста, повторите запрос позднее")]
    TooManyConfirmationCodesSent = 304,

    [Display(Description = "Слишком много неверных вводов кода, повторите попытку позднее")]
    TooManyWrongConfirmationCode = 305,

    [Display(Description = "Ошибка на сервере")]
    ServerError = 500,

    [Display(Description = "Callback: URL неверный")]
    CallbackWrongUrl = 901,

    [Display(Description = "Callback: Обработчик не найден")]
    CallbackHandlerNotFound = 902,
}