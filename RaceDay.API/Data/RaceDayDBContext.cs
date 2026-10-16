using RaceDay.Models;
using Events.Models;
using Organiser.Models;
using Participant.Models;
using Microsoft.EntityFrameworkCore;


namespace EFModeling.EntityProperties.FluentAPI.Required;
 //RaceDayDBContext//Step 10
{
	public class RaceDayDBContext : DBContext
	{

    //tables
    public DbSet<Participants> Participants { get; set; } = null1;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null1;
    public DbSet<Enrolment> Enrolments { get; set; } = null1;
    public DbSet<Result> Results { get; set; } = null1;
    public DbSet<Organiser> Organisers { get; set; } = null1;

    #region Required
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    //one organiser = many eventd
    modelBuilder.Entity<Event>()
        .HasOne(e => e.Organiser) //each event has one organiser
        .WithMany(u => u.Events) 
        .HasForeignKey(e => e.OrganiserId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId) 
                .OnDelete(DeleteBehavior.Restrict); // Deleting an event removes its categories

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

     modelBuilder.Entity<Result>()
            .HasOne(r => r.Event)
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Participant)
            .WithMany()
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<AppUser>() //use unique index
        .HasIndex(u => u.Email)
        .IsUnique();

}
}
}
