namespace Fab.UseCases.Support.Sorts;

public interface IMultipleSortingRequest
{
    /// <summary>
    ///     Сортировка по нескольким полям
    ///
    ///     /!\ Алярм: SwaggerUI формирует неправильный запрос.
    ///     Для передачи массива объектов через query параметр используйте:
    ///
    ///     `?sorts[0].field=createdAt&amp;sorts[0].direction=desc&amp;sorts[1].field=relation.innerField&amp;sorts[1].direction=asc`
    /// </summary>
    public ICollection<Sorting>? Sorts { get; }
}