namespace Idam.EFTimestamps.Tests.Tests;
internal class SoftDeleteTests : BaseTest
{
    //[Fact]
    //public async Task Should_SetDeletedAt_OnSoftDelete()
    //{
    //    Boo data = await AddAsync(this._booFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Boo? dataFromDb = await this._context.Boos
    //        .IgnoreQueryFilters()
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.NotNull(dataFromDb);
    //    _ = Assert.NotNull(dataFromDb.DeletedAt);
    //    Assert.True(dataFromDb.Trashed());
    //}

    //[Fact]
    //public async Task When_SoftDeleted_ShouldClearDataFromNewList()
    //{
    //    Boo data = await AddAsync(this._booFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Boo? dataFromDb = await this._context.Boos
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.Null(dataFromDb);
    //}

    //[Fact]
    //public async Task Should_RestoreSoftDeletedEntity()
    //{
    //    Boo data = await AddAsync(this._booFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Boo? dataFromDb = await this._context.Boos
    //        .IgnoreQueryFilters()
    //        .Where(w => w.Id.Equals(data.Id))
    //        .Where(w => w.DeletedAt.HasValue)
    //        .FirstOrDefaultAsync();

    //    Assert.NotNull(dataFromDb);
    //    _ = Assert.NotNull(dataFromDb.DeletedAt);

    //    _ = this._context.Boos.Restore(dataFromDb);
    //    _ = await this._context.SaveChangesAsync();

    //    dataFromDb = await this._context.Boos
    //        .FirstOrDefaultAsync(w => w.Id.Equals(dataFromDb.Id));

    //    Assert.NotNull(dataFromDb);
    //    Assert.Null(dataFromDb.DeletedAt);
    //    Assert.False(dataFromDb.Trashed());
    //}

    //[Fact]
    //public async Task Should_PerformPermanentDelete_AfterSoftDelete()
    //{
    //    Boo data = await AddAsync(this._booFaker.Generate());
    //    data = await DeleteAsync(data);
    //    data = await DeleteAsync(data);

    //    Boo? dataFromDb = await this._context.Boos
    //        .IgnoreQueryFilters()
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.Null(dataFromDb);
    //}

    //[Fact]
    //public async Task Should_SetDeletedAt_OnUnixSoftDelete()
    //{
    //    Foo data = await AddAsync(this._fooFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Foo? dataFromDb = await this._context.Foos
    //        .IgnoreQueryFilters()
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.NotNull(dataFromDb);
    //    _ = Assert.NotNull(dataFromDb.DeletedAt);
    //    Assert.True(dataFromDb.Trashed());
    //}

    //[Fact]
    //public async Task When_SoftDeleted_ShouldClearUnixDataFromNewList()
    //{
    //    Foo data = await AddAsync(this._fooFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Foo? dataFromDb = await this._context.Foos
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.Null(dataFromDb);
    //}

    //[Fact]
    //public async Task Should_PerformPermanentDelete_AfterUnixSoftDelete()
    //{
    //    Foo data = await AddAsync(this._fooFaker.Generate());
    //    data = await DeleteAsync(data);
    //    data = await DeleteAsync(data);

    //    Foo? dataFromDb = await this._context.Foos
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.Null(dataFromDb);
    //}

    //[Fact]
    //public async Task Should_RestoreUnixSoftDeletedEntity()
    //{
    //    Foo data = await AddAsync(this._fooFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Foo? dataFromDb = await this._context.Foos
    //        .IgnoreQueryFilters()
    //        .Where(w => w.Id.Equals(data.Id))
    //        .Where(w => w.DeletedAt.HasValue)
    //        .FirstOrDefaultAsync();

    //    Assert.NotNull(dataFromDb);
    //    _ = Assert.NotNull(dataFromDb.DeletedAt);

    //    _ = this._context.Foos.Restore(dataFromDb);
    //    _ = await this._context.SaveChangesAsync();

    //    dataFromDb = await this._context.Foos
    //        .FirstOrDefaultAsync(w => w.Id.Equals(dataFromDb.Id));

    //    Assert.NotNull(dataFromDb);
    //    Assert.Null(dataFromDb.DeletedAt);
    //    Assert.False(dataFromDb.Trashed());
    //}

    //[Fact]
    //public async Task When_ForceDelete_ShouldPerformPermanentDelete()
    //{
    //    Boo data = await AddAsync(this._booFaker.Generate());
    //    _ = this._context.Boos.ForceRemove(data);
    //    _ = await this._context.SaveChangesAsync();

    //    Boo? dataFromDb = await this._context.Boos
    //        .IgnoreQueryFilters()
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.Null(dataFromDb);
    //}

    //[Fact]
    //public async Task Should_Throw_When_DataTypeOfTimeStampsFieldIsInvalid()
    //{
    //    Doo data = await AddAsync(this._dooFaker.Generate());

    //    _ = await Assert.ThrowsAsync<InvalidCastException>(() => DeleteAsync(data));
    //}

    //[Fact]
    //public async Task When_SoftDeleted_Should_SetDateTimeKindUtc_OnDeletedAt()
    //{
    //    Goo data = await AddAsync(this._gooFaker.Generate());
    //    data = await DeleteAsync(data);

    //    Goo? dataFromDb = await this._context.Goos
    //        .IgnoreQueryFilters()
    //        .FirstOrDefaultAsync(x => x.Id.Equals(data.Id));

    //    Assert.NotNull(dataFromDb);
    //    _ = Assert.NotNull(dataFromDb.DeletedAt);
    //    Assert.True(dataFromDb.Trashed());
    //    Assert.True(dataFromDb.DeletedAt!.Value.Kind.Equals(DateTimeKind.Utc));
    //}
}
