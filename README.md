# Fortuna.NET

This is an implementation of the Fortuna PRNG algorithm for .NET. 
It has been built on _NETStandard 1.3_, and so is compatible with .NET Framework 4.6 as well as .NET Core 1.0 or higher (on Mac, Linux and Windows).

## What is Fortuna?

Fortuna is a cryptographically secure pseudo-random number generator. You can [read the published specification here](https://www.schneier.com/academic/paperfiles/fortuna.pdf).

## Design

Per Wikipedia: 

Fortuna is a family of secure PRNGs; its design leaves some choices open to implementors. It is composed of the following pieces:
* The generator itself, which once seeded will produce an indefinite quantity of pseudo-random data.
* The entropy accumulator, which collects genuinely random data from various sources and uses it to reseed the generator when enough new randomness has arrived.
* The seed file, which stores enough state to enable the computer to start generating random numbers as soon as it has booted.

## Usage

You can download the package on [nuget.org](https://www.nuget.org/packages/Fortuna).

It is recommended that you use one of the provided factory methods to construct the provider:

``` var rng = PRNGFortunaProviderFactory.Create(); ```

There is also an asynchronous override provided:

``` 
var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;
var rng = await PRNGFortunaProviderFactory.CreateAsync(token); 
```

The latter will allow you to create the PRNG asynchronously - this may be preferable, as depending on external conditions, 
it may take a while before there is sufficient random data accumulated before the PRNG is ready for use.

Regardless of the method used to construct the PRNG, you can request random data via the following method:
``` 
var data = new byte[128];
var randomData = prng.GetBytes(data);
```

If you are only interested in selecting a random integer greater than or equal to 0, you can use the following extension method:

``` int randNumber = prng.RandomNumber(10); ```

This will return a number in the range of [0, 10)

## Implementation

### Sources of Entropy
The architecture of this particular implementation explicitly allows for adding new sources of entropy.  The following sources are provided out of the box:
* .NET CryptoServiceProvider
* Environment Uptime
* Garbage Collector Statistics
* Current Process Statistics
* System Time

New providers can be plugged in as needed, up to a maximum of 255 total.

### Entropy Scheduler
This implementation uses a basic event-based scheduler to periodically sample entropy providers for new data.

Entropy providers have the latitude to include up to as much as 32 bytes of entropy, and can dictate the sample period.
