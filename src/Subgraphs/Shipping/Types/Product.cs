namespace Demo.Shipping.Types;

public sealed record Product([property: ID<Product>] int Id)
{
    public int GetDeliveryEstimate(
        string zip,
        [Require(
            """
            {
              weight,
              length: dimension.length
              width: dimension.width
              height: dimension.height
            }
            """)]
        ProductDimensionInput dimension)
    {
        // Base delivery time starts at 2 days for local processing
        const int baseDays = 2;
        
        // Calculate volume in cubic centimeters (assuming dimensions are in cm)
        var volumeCm3 = dimension.Length * dimension.Width * dimension.Height;
        
        // Distance factor based on ZIP code (simplified simulation)
        var distanceDays = GetDistanceDays(zip);
        
        // Size and weight factors
        var sizeDays = GetSizeDelayDays(volumeCm3, dimension.Weight);
        
        // Add some realistic randomness (±1 day) based on ZIP code
        var variabilityDays = GetVariabilityDays(zip);
        
        // Calculate total with minimum of 1 day
        var totalDays = Math.Max(1, baseDays + distanceDays + sizeDays + variabilityDays);
        
        // Cap at reasonable maximum (14 days for demo purposes)
        return Math.Min(14, totalDays);
    }
    
    private static int GetDistanceDays(string zip)
    {
        // Simulate distance based on ZIP code patterns
        // This creates consistent results for the same ZIP
        var zipHash = zip.GetHashCode();
        var normalizedHash = Math.Abs(zipHash % 100);
        
        return normalizedHash switch
        {
            // 20% local (same day processing)
            < 20 => 0,
            
            // 30% regional (1 extra day)
            < 50 => 1,
            
            // 25% medium distance
            < 75 => 2,
            
            // 15% far
            < 90 => 3,  
            
            // 10% very far
            _ => 4
        };
    }
    
    private static int GetSizeDelayDays(double volumeCm3, int weightGrams)
    {
        // Large or heavy items take longer to process and ship
        var sizeDays = 0;

        // Volume-based delays
        switch (volumeCm3)
        {
            // > 100L (large furniture, appliances)
            case > 100000:
                sizeDays += 3;
                break;
            
            // > 50L (medium furniture)
            case > 50000:
                sizeDays += 2;
                break;
            
            // > 10L (small furniture, large electronics)
            case > 10000:
                sizeDays += 1;
                break;
        }

        // Weight-based delays
        switch (weightGrams)
        {
            // > 20kg
            case > 20000:
                sizeDays += 2;
                break;
            
            // > 10kg
            case > 10000:
                sizeDays += 1;
                break;
        }
        
        return sizeDays;
    }
    
    private static int GetVariabilityDays(string zip)
    {
        // Add realistic variability that's consistent per ZIP code
        // This simulates real-world factors like local carrier efficiency,
        // weather patterns, etc.
        var seed = zip.GetHashCode();
        var random = new Random(Math.Abs(seed));
        
        // 60% chance of no change, 25% chance of +1 day, 15% chance of -1 day
        var roll = random.Next(100);
        return roll switch
        {
            // Lucky, faster than expected
            < 15 => -1,  
            
            // Right on schedule
            < 60 => 0,   
            
            // Slight delay
            _ => 1       
        };
    }
}