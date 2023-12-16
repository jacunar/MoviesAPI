﻿using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entidades;

namespace MoviesAPI; 
public class ApplicationDbContext: DbContext {
	public ApplicationDbContext(DbContextOptions options): base(options) {
	}

	public DbSet<Genre>	Genres { get; set; }
}