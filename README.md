# Nexus.Link.AsyncCaller.Distribution
Code meant to be deployed on an Azure Function App, listening to Azure Storage Queues.

We use `SchemaVersion` 1 in Nexus Async Caller, which means the queue names are fixed (you can find them in Functions.cs).

To get going, see [Nexus Async Caller Setup Guide](https://docs.nexus.link/docs/nexus-async-caller-setup-1-5-onwards).

## Overview
The purpose of this code is to make it easy for Nexus customers to handle the asynchronous requests of Async Caller.
The idea is that the ICC sets up CI/CD pipelines that fetch code from this repository and deploy it onto a Azure function app in their Azure subscription.

## Queue triggered functions
There is one function triggering on the AC "standard" queue ("async-caller-standard-queue"), where requests without a specified priority are put.

Then there are a number of functions triggering on the AC priority queues ("async-caller-priorityX-queue").
There should be enough of them for everyone's needs, but if there are too few, contact Nexus support.

## Request distribution
Messages on the queues are interpreted as AC RequestEnvelopes and the AC SDK is used to distribute the requests.

## Required settings
The function app needs a few app settings to work:

| Name                              | Value                                                        |
| --------------------------------- | ------------------------------------------------------------ |
| AzureWebJobsStorage               | Connection string to an Azure Storage account                |
| Nexus:Organization                | Organization part of the tenant that this function app is running in |
| Nexus:Environment                 | Environment part of the tenant that this function app is running in |
| Nexus:FundamentalsUrl             | Usually "https://fundamentals-svc.nexus.link" for production and "https://prdsim-fulcrum-fundamentals.azurewebsites.net" for non-production |
| Nexus:Authentication:ClientId     | Authentication for fetching Async Caller and Logging configuration from Nexus Fundamentals |
| Nexus:Authentication:ClientSecret | Authentication for fetching Async Caller and Logging configuration from Nexus Fundamentals |
| Nexus:RunTimeLevel                | RunTimeLevelEnum telling which type of environment the function app is in.<br />Defaults to "Production". |

## Logging

This code supports standard Nexus logging. It also uses the Azure Functions standard ILogger in it's own code (but this is not used in the AC SDK which it heavily depends on).