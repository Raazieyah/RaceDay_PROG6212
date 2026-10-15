using System;
using Microsoft.EntityFrameworkCore;
{
    
}

namespace EFModeling.EntityProperties.FluentAPI.Required;
 //RaceDayDBContext//Step 10
{
	public DbContext()
	{
       // public R5aceDayDbContext(DbContextOptions{}
    public DbSet<Participants> Participants { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Enrolment> Enrolments { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<Organiser> Organiser { get; set; }

    #region Required
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    //one organiser = many eventd
    modelBuilder.Entity<Event>()
        .HasOne(e => e.Organiser) //each event has one organiser
        .WithMany(u => u.Events) //.WithMany(u => u.Events)
        .HasForeignKey(e => e.Organiser)
        .OnDelete(DeleteBehavior.Restrict); // Prevents accidental cascade cycles

    modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting an event removes its categories

    // 4. One Participant -> Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Participant)
        .WithMany(p => p.Enrolments)
        .HasForeignKey(en => en.ParticipantId)
        .OnDelete(DeleteBehavior.Restrict);

    // 5. One Event -> Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Event)
        .WithMany(e => e.Enrolments)
        .HasForeignKey(en => en.EventId)
        .OnDelete(DeleteBehavior.Restrict);

    // 6. One Category -> Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Category)
        .WithMany(c => c.Enrolments)
        .HasForeignKey(en => en.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    // 7. One Enrolment -> Zero or One Result (One-to-One / Optional)
    modelBuilder.Entity<Result>()
        .HasOne(r => r.Enrolment)
        .WithOne(en => en.Result)
        .HasForeignKey<Result>(r => r.EnrolmentId)
        .OnDelete(DeleteBehavior.Cascade);

    // Example of configuring a Unique Index (e.g., Email or unique Participant per Event)
    modelBuilder.Entity<AppUser>()
        .HasIndex(u => u.Email)
        .IsUnique();

}
}
}
