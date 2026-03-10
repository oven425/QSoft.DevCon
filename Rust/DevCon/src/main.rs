#[cfg(windows)]
use winapi::um::setupapi::SetupDiClassGuidsFromNameA;
#[cfg(windows)]
use winapi::shared::guiddef::GUID;
#[cfg(windows)]
use std::ffi::CString;
#[cfg(windows)]
use std::mem;

/// Wrapper for SetupDiClassGuidsFromName to retrieve class GUIDs by class name
#[cfg(windows)]
fn get_class_guids_from_name(class_name: &str) -> Result<Vec<GUID>, String> {
    unsafe {
        let class_name_cstr = CString::new(class_name)
            .map_err(|e| format!("Failed to create CString: {}", e))?;
        
        // First call to get the required buffer size
        let mut guid_count: u32 = 0;
        let _result = SetupDiClassGuidsFromNameA(
            class_name_cstr.as_ptr() as *const i8,
            std::ptr::null_mut(),
            0,
            &mut guid_count,
        );
        
        if guid_count == 0 {
            return Err(format!("No GUIDs found for class: {}", class_name));
        }
        
        // Allocate buffer for GUIDs
        let mut guids = vec![mem::zeroed::<GUID>(); guid_count as usize];
        
        // Second call to get the actual GUIDs
        let result = SetupDiClassGuidsFromNameA(
            class_name_cstr.as_ptr() as *const i8,
            guids.as_mut_ptr(),
            guid_count,
            &mut guid_count,
        );
        
        if result == 0 {
            return Err(format!("SetupDiClassGuidsFromName failed for class: {}", class_name));
        }
        
        Ok(guids)
    }
}

fn main() {
    #[cfg(windows)]
    {
        // Example: Get GUIDs for "Net" class (Network adapters)
        match get_class_guids_from_name("Net") {
            Ok(guids) => {
                println!("Found {} GUID(s) for Net class:", guids.len());
                for (i, guid) in guids.iter().enumerate() {
                    let guid_string = format!("{{{:08X}-{:04X}-{:04X}-{:02X}{:02X}-{:02X}{:02X}{:02X}{:02X}{:02X}{:02X}}}", 
                        guid.Data1,
                        guid.Data2,
                        guid.Data3,
                        guid.Data4[0], guid.Data4[1],
                        guid.Data4[2], guid.Data4[3], guid.Data4[4], guid.Data4[5], guid.Data4[6], guid.Data4[7]
                    );
                    println!("  GUID {}: {}", i + 1, guid_string);
                }
            }
            Err(e) => eprintln!("Error: {}", e),
        }
    }
    
    #[cfg(not(windows))]
    {
        println!("This application requires Windows with Setup API support");
    }
}
