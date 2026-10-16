using System;
using Microsoft.EntityFrameworkCore;
{
    
}

namespace EFModeling.EntityProperties.FluentAPI.Required;
 //RaceDayDBContext//Step 10
{
	public DbContext()
	{
       // public RaceDayDbContext(DbContextOptions{}
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
        .WithMany(u => u.Events) 
        .HasForeignKey(e => e.Organiser)
        .OnDelete(DeleteBehavior.Restrict); 

    modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting an event removes its categories

    //  One Participant = Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Participant)
        .WithMany(p => p.Enrolments)
        .HasForeignKey(en => en.ParticipantId)
        .OnDelete(DeleteBehavior.Restrict);

    //  One Event = Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Event)
        .WithMany(e => e.Enrolments)
        .HasForeignKey(en => en.EventId)
        .OnDelete(DeleteBehavior.Restrict);

    // One Category = Many Enrolments
    modelBuilder.Entity<Enrolment>()
        .HasOne(en => en.Category)
        .WithMany(c => c.Enrolments)
        .HasForeignKey(en => en.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    // One Enrolment = Zero or One Result 
    modelBuilder.Entity<Result>()
        .HasOne(r => r.Enrolment)
        .WithOne(en => en.Result)
        .HasForeignKey<Result>(r => r.EnrolmentId)
        .OnDelete(DeleteBehavior.Cascade);

    
    modelBuilder.Entity<AppUser>() //use unique index
        .HasIndex(u => u.Email)
        .IsUnique();

}
}
}
