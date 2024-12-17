import math

def inventory_reordering(items):
    """
    Function to determine a reordering plan for warehouse inventory.
    
    Args:
    items (list of dict): A list containing inventory data. Each item is a dictionary with:
        - item_id (str): Unique identifier for the item.
        - current_stock (int): Current inventory level.
        - forecasted_demand (int): Forecasted demand for the next period.
        - reorder_cost_per_unit (float): Cost incurred for ordering a single unit of the item.
        - batch_size (int): Fixed batch size for reordering.
    
    Returns:
    list of tuples: A list containing the reordering plan in the format:
                    (item_id, units_to_order)
    """
    reorder_plan = []
    
    for item in items:
        item_id = item['item_id']
        current_stock = item['current_stock']
        forecasted_demand = item['forecasted_demand']
        reorder_cost_per_unit = item['reorder_cost_per_unit']
        batch_size = item['batch_size']
        
        # Step 1: Calculate shortage
        shortage = max(0, forecasted_demand - current_stock)
        
        if shortage > 0:
            # Step 2: Calculate batches needed (round up to nearest batch)
            batches_needed = math.ceil(shortage / batch_size)
            units_to_order = batches_needed * batch_size
            
            # Step 3: Append result to the reorder plan
            reorder_plan.append((item_id, units_to_order))
    
    return reorder_plan

# Test the algorithm with sample data
if __name__ == "__main__":
    items = [
        {"item_id": "A", "current_stock": 50, "forecasted_demand": 120, "reorder_cost_per_unit": 5, "batch_size": 10},
        {"item_id": "B", "current_stock": 80, "forecasted_demand": 70, "reorder_cost_per_unit": 3, "batch_size": 20},
        {"item_id": "C", "current_stock": 20, "forecasted_demand": 100, "reorder_cost_per_unit": 4, "batch_size": 15}
    ]
    
    reordering_plan = inventory_reordering(items)
    print("Reordering Plan:")
    for item_id, units in reordering_plan:
        print(f"Item {item_id}: Order {units} units")
