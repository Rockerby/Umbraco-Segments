# Umbraco Segments
A small demo of getting started with segments in Umbraco 10.

## How to get started
Open `Segments.sln` and run the `Website` project or simply type the below from the command line in the project root:
```
dotnet run --project Website
``` 

## What's going on?
Segments are a feature within Umbraco that have existed for a while, but are hidden.

Firstly we need to enable segments (done in `SegmentHelper.Events.AllowSegmentationEvent`)

Then we get the option within the back office to enable Segments.

After creating a page you then have the same UI as with multi-languages to change segment data (drop-down in name bar)

A test page has been setup. Visit https://localhost:44369/blog/segment to see the regular page and then https://localhost:44369/blog/segment?segment to see the segmented page. Here's a quick demo of it in action:

![chrome_cdZfGgX1fI](https://github.com/Rockerby/Umbraco-Segments/assets/5808078/734b4393-5361-4d8e-9d4d-52797f9ce44d)

## Issues
- There's a few issues with segments, mainly centered around the UI within Umbraco itself. I've found issues when pages have been created that you need to refresh them in order to see the segment options.
- New segments can't be added to existing nodes, likely fixed by altering `SegmentHelper.Events.CreateSegmentsEvent`.
- Multi-lingual hasn't been tested.
- Each property on a doc type gets an overlay warning the user it will change for all segments - this doesn't happen for language variants and is poor UX.
